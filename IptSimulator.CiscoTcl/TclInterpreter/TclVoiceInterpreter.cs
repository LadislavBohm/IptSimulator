using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Commands;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Model.InputData;
using IptSimulator.CiscoTcl.TclInterpreter.Commands;
using IptSimulator.CiscoTcl.TclInterpreter.EventArgs;
using IptSimulator.CiscoTcl.Utils;
using NLog;

namespace IptSimulator.CiscoTcl.TclInterpreter
{
    public sealed class TclVoiceInterpreter : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<int, int> _activeBreakpoints = new ConcurrentDictionary<int, int>();
        private readonly ConcurrentQueue<ICommand> _commandStack = new ConcurrentQueue<ICommand>();
        private readonly TimeSpan _commandEvaluateTimeout = TimeSpan.FromMilliseconds(100);

        private IEnumerable<VariableWithValue> _watchVariables;
        private readonly object _lockRoot = new object();
        private readonly Interpreter _interpreter;
        private int? _breakpointLineNumber;
        private bool _pauseOnBreakpoint;
        private bool _pauseOnActiveBreakpoint = true;
        private bool _delayExecution = false;
        private bool _delayingExecution;
        private TimeSpan _executionDelay = TimeSpan.FromMilliseconds(250);
        private string _currentState;
        private string _currentEvent;


        #region Creation

        private TclVoiceInterpreter(Interpreter interpreter)
        {
            _interpreter = interpreter;
            Result result = null;

            var customCommands = TclCommandProvider.GetCustomCommands();
            Logger.Info("Adding custom TCL commands.");
            foreach (var customCommand in customCommands)
            {
                long token = 0;
                Logger.Debug($"Adding {customCommand.Name} command.");
                var code = interpreter.AddCommand(customCommand, null, ref token, ref result);

                if (code != ReturnCode.Ok)
                {
                    Logger.Warn($"Failed to add {customCommand.Name} command. Error: {result}");
                }

                SubscribeToInputCommand(customCommand as CiscoTclCommand);

                var breakpoint = customCommand as Breakpoint;
                if (breakpoint == null) continue;

                breakpoint.BreakpointHit += OnBreakpointHit;
            }

            void SubscribeToInputCommand(CiscoTclCommand command)
            {
                if (command == null) return;
                if (command is IInputRequestingCommand<DigitsInputData> digitInput)
                {
                    digitInput.OnInputRequested += DigitInputOnOnInputRequested;
                }
                foreach (var subCommand in command.TclSubCommands.OfType<IInputRequestingCommand<DigitsInputData>>())
                {
                    subCommand.OnInputRequested += DigitInputOnOnInputRequested;
                }
            }
        }

        /// <summary>
        /// Factory method to create and properly initialize TCL interpreter. 
        /// </summary>
        /// <returns>Initialized TCL interpreter</returns>
        public static TclVoiceInterpreter Create(CancellationTokenSource cancellationToken)
        {
            Logger.Info("Creating TCL interpreter.");

            Result result = null;
            var tclInterpreter = Interpreter.Create(null, CreateFlags.Default, ref result);

            Logger.Debug("TCL Interpreter created.");

            var interpreter = new TclVoiceInterpreter(tclInterpreter);

            interpreter.StartAsync(cancellationToken);

            Logger.Info("TCL interpreter successfully initialized.");

            return interpreter;
        }

        #endregion

        #region Properties

        private bool PauseOnBreakpoint
        {
            get { lock (_lockRoot) return _pauseOnBreakpoint; }
            set
            {
                lock (_lockRoot)
                {
                    _pauseOnBreakpoint = value;
                }
                RaiseDebugModeChangedEvent(value, DelayingExecution, _breakpointLineNumber);
            }
        }

        private bool DelayingExecution
        {
            get { lock(_lockRoot) return _delayingExecution; }
            set
            {
                lock (_lockRoot)
                {
                    _delayingExecution = value;
                }
                RaiseDebugModeChangedEvent(PauseOnBreakpoint, value, _breakpointLineNumber);
            }
        }

        private bool PauseOnActiveBreakpoint
        {
            get { lock (_lockRoot) return _pauseOnActiveBreakpoint; }
            set { lock (_lockRoot) _pauseOnActiveBreakpoint = value; }
        }

        private bool DelayExecution
        {
            get { lock (_lockRoot) return _delayExecution; }
            set { lock (_lockRoot) _delayExecution = value; }
        }

        private TimeSpan ExecutionDelay
        {
            get { lock (_lockRoot) return _executionDelay; }
            set { lock (_lockRoot) _executionDelay = value; }
        }

        public IEnumerable<VariableWithValue> WatchVariables
        {
            get { lock (_lockRoot) return _watchVariables; }
            private set { lock (_lockRoot) _watchVariables = value; RaiseWatchVariablesChangedEvent(); }
        }

        public string CurrentState
        {
            get { lock (_lockRoot) return _currentState; }
            private set { lock (_lockRoot) _currentState = value; }
        }

        public string CurrentEvent
        {
            get { lock (_lockRoot) return _currentEvent; }
            private set { lock (_lockRoot) _currentEvent = value; }
        }

        #endregion

        #region Public methods

        public void Evaluate(string script, IEnumerable<int> breakpoints)
        {
            SetBreakpoints(breakpoints.ToArray());
            _commandStack.Enqueue(new EvaluteScriptCommand(script));
        }

        public void Evaluate(string script)
        {
            _commandStack.Enqueue(new EvaluteScriptCommand(script));
        }

        public void Continue()
        {
            PauseOnActiveBreakpoint = true;
            PauseOnBreakpoint = false;
            _breakpointLineNumber = null;
        }

        public void StepInto()
        {
            PauseOnActiveBreakpoint = false;
            PauseOnBreakpoint = false;
            _breakpointLineNumber = null;
        }

        public void AddBreakpoint(int lineNumber)
        {
            if (!_activeBreakpoints.ContainsKey(lineNumber))
            {
                _activeBreakpoints.TryAdd(lineNumber, lineNumber);
            }
        }

        public void RemoveBreakpoint(int lineNumber)
        {
            if (_activeBreakpoints.ContainsKey(lineNumber))
            {
                int removed;
                _activeBreakpoints.TryRemove(lineNumber, out removed);
            }
        }

        public void ReplaceBreakpoints(IEnumerable<int> newBreakpoints)
        {
            SetBreakpoints(newBreakpoints.ToArray());
        }

        public void ResetBreakpoints()
        {
            _activeBreakpoints.Clear();
        }

        public void DisableDelayedExecution()
        {
            DelayExecution = false;
        }

        public void EnableDelayedExecution(TimeSpan delay)
        {
            ExecutionDelay = delay;
            DelayExecution = true;
        }

        #endregion

        #region Private methods

        private void SetBreakpoints(params int[] breakpoints)
        {
            _activeBreakpoints.Clear();
            if (breakpoints != null)
            {
                foreach (int breakpoint in breakpoints)
                {
                    _activeBreakpoints.TryAdd(breakpoint, breakpoint);
                }
            }
        }

        private async Task StartAsync(CancellationTokenSource cancellationToken)
        {
            await Task.Run(async () =>
            {
                Logger.Debug("Starting interpreter.");

                while (!cancellationToken.IsCancellationRequested)
                {
                    ICommand command;
                    if (_commandStack.TryDequeue(out command))
                    {
                        command.Evaluate(this);
                        RefreshCurrentVariables();
                    }

                    await Task.Delay(_commandEvaluateTimeout);
                }
            });
        }

        internal void EvaluateScript(string script)
        {
            Result result = null;
            int errorLine = 0;
            var code = _interpreter.EvaluateScript(script, ref result, ref errorLine);

            Logger.Info($"Script evaluted with code {code}.");
            if (code == ReturnCode.Ok)
            {
                Logger.Debug($"Evaluation result: {result}");
            }
            else
            {
                Logger.Error($"Error line: {errorLine}, error message: {result}");
            }
            RaiseEvaluateCompletedEvent(code, result, errorLine);
        }

        private void RefreshCurrentVariables()
        {
            try
            {
                Logger.Debug("Refreshing current variables and their values.");
                WatchVariables = TclUtils.GetVariableValues(_interpreter, true);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while refreshing current variables.");
            }
        }

        private void OnBreakpointHit(object sender, BreakpointHitEventArgs eventArgs)
        {
            _breakpointLineNumber = eventArgs.LineNumber;
            if (_pauseOnActiveBreakpoint && !_activeBreakpoints.ContainsKey(eventArgs.LineNumber))
            {
                if (!DelayExecution)
                {
                    _breakpointLineNumber = null;
                    Logger.Info("Breakpoint is no longer in active breakpoints, continuing execution.");
                }
                else
                {
                    Logger.Info( $"Breakpoint not in active breakpoints, delaying execution for {ExecutionDelay.TotalMilliseconds} milliseconds.");
                    RefreshCurrentVariables();
                    DelayingExecution = true;
                    Thread.Sleep(ExecutionDelay);
                    DelayingExecution = false;
                }
            }
            else
            {
                PauseOnBreakpoint = true;
                Logger.Info("Stopping at breakpoint.");
                RefreshCurrentVariables();

                while (PauseOnBreakpoint)
                {
                    Logger.Trace($"Sleeping on breakpoint for {_commandEvaluateTimeout.TotalMilliseconds} milliseconds.");
                    Thread.Sleep(_commandEvaluateTimeout);
                }

                Logger.Info("Continuing from breakpoint.");
            }
        }

        private void DigitInputOnOnInputRequested(object sender, InputEventArgs<DigitsInputData> inputEventArgs)
        {
            RaiseOnInputDigitsRequestedEvent(inputEventArgs);
        }

        #endregion

        #region Events

        private void RaiseDebugModeChangedEvent(bool isBreakpointHit, bool isDealayedLineHit, int? lineNumber) =>
            BreakpointHitChanged?.Invoke(this, new DebugModeEventArgs(isBreakpointHit, isDealayedLineHit, lineNumber));

        private void RaiseWatchVariablesChangedEvent() => WatchVariablesChanged?.Invoke(this, System.EventArgs.Empty);

        private void RaiseEvaluateCompletedEvent(ReturnCode returnCode, Result result, int errorLine = 0) =>
            EvaluateCompleted?.Invoke(this, new EvaluteResultEventArgs(result, returnCode, errorLine));

        private void RaiseOnInputDigitsRequestedEvent(InputEventArgs<DigitsInputData> inputEventArgs)
        {
            OnInputDigitsRequested?.Invoke(this, inputEventArgs);
        }

        public event EventHandler<DebugModeEventArgs> BreakpointHitChanged;
        public event EventHandler<System.EventArgs> WatchVariablesChanged;
        public event EventHandler<EvaluteResultEventArgs> EvaluateCompleted;

        /// <summary>
        /// Input digits requested by TCL command. Don't forget to call SetInputData method to pass collected digits.
        /// </summary>
        public event EventHandler<InputEventArgs<DigitsInputData>> OnInputDigitsRequested;

        public void Dispose()
        {
            _interpreter?.Dispose();
        }

        #endregion
    }
}
