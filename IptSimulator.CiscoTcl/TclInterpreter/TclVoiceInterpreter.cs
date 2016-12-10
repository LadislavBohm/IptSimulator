using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Commands;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.TclInterpreter.Commands;
using IptSimulator.CiscoTcl.TclInterpreter.EventArgs;
using IptSimulator.CiscoTcl.Utils;
using NLog;

namespace IptSimulator.CiscoTcl.TclInterpreter
{
    public sealed class TclVoiceInterpreter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Eagle._Components.Public.Interpreter _interpreter;
        private readonly TimeSpan _commandEvaluateTimeout = TimeSpan.FromMilliseconds(100);
        private readonly ConcurrentQueue<ICommand> _commandStack = new ConcurrentQueue<ICommand>();
        private readonly object _lockRoot = new object();
        private bool _pauseOnBreakpoint;
        private IEnumerable<VariableWithValue> _watchVariables;
        private string _currentState;
        private string _currentEvent;

        #region Creation

        private TclVoiceInterpreter(Eagle._Components.Public.Interpreter interpreter)
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

                var breakpoint = customCommand as Breakpoint;
                if (breakpoint == null) continue;

                breakpoint.BreakpointHit += OnBreakpointHit;
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
            var tclInterpreter = Eagle._Components.Public.Interpreter.Create(null, CreateFlags.Default, ref result);

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
            get
            {
                lock (_lockRoot)
                {
                    return _pauseOnBreakpoint;
                }

            }
            set
            {
                lock (_lockRoot)
                {
                    _pauseOnBreakpoint = value;
                }
                RaiseDebugModeChangedEvent(value);
            }
        }

        public IEnumerable<VariableWithValue> WatchVariables
        {
            get
            {
                lock (_lockRoot)
                {
                    return _watchVariables;
                }
            }
            private set
            {
                lock (_lockRoot)
                {
                    _watchVariables = value;
                }
                RaiseWatchVariablesChangedEvent();
            }
        }

        public string CurrentState
        {
            get
            {
                lock (_lockRoot)
                {
                    return _currentState;
                }
            }
            private set
            {
                lock (_lockRoot)
                {
                    _currentState = value;
                }
            }
        }

        public string CurrentEvent
        {
            get
            {
                lock (_lockRoot)
                {
                    return _currentEvent;
                }
            }
            private set
            {
                lock (_lockRoot)
                {
                    _currentEvent = value;
                }
            }
        }

        #endregion

        #region Public methods

        public void Evaluate(string script)
        {
            _commandStack.Enqueue(new EvaluteScriptCommand(script));
        }

        public void Continue()
        {
            PauseOnBreakpoint = false;
        }

        #endregion

        #region Private methods

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

        private void OnBreakpointHit(object sender, System.EventArgs eventArgs)
        {
            //breakpoint hit, stop here
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


        #endregion

        #region Events

        private void RaiseDebugModeChangedEvent(bool isBreakpointHit)
        {
            DebugModeChanged?.Invoke(this, new DebugModeEventArgs(isBreakpointHit));
        }

        private void RaiseWatchVariablesChangedEvent()
        {
            WatchVariablesChanged?.Invoke(this, System.EventArgs.Empty);
        }

        private void RaiseEvaluateCompletedEvent(ReturnCode returnCode, Result result, int errorLine = 0)
        {
            EvaluateCompleted?.Invoke(this, new EvaluteResultEventArgs(result, returnCode, errorLine));
        }

        public event EventHandler<DebugModeEventArgs> DebugModeChanged;
        public event EventHandler<System.EventArgs> WatchVariablesChanged;
        public event EventHandler<EvaluteResultEventArgs> EvaluateCompleted;

        #endregion
    }
}
