using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eagle._Components.Public;
using NLog;

namespace IptSimulator.CiscoTcl.Interpreter
{
    public sealed class TclVoiceInterpreter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Eagle._Components.Public.Interpreter _interpreter;
        private readonly TimeSpan _commandEvalTimeout = TimeSpan.FromMilliseconds(100);
        private readonly ConcurrentStack<ICommand> _commandStack = new ConcurrentStack<ICommand>();
        private readonly object _lockRoot = new object();
        private bool _isBreakpointHit = false;

        #region Creation

        private TclVoiceInterpreter(Eagle._Components.Public.Interpreter interpreter)
        {
            _interpreter = interpreter;
            _interpreter.InteractiveLoopCallback += InteractiveLoopCallback;
        }

        /// <summary>
        /// Factory method to create and properly initialize TCL interpreter. 
        /// </summary>
        /// <returns>Initialized TCL interpreter</returns>
        public static TclVoiceInterpreter Create(CancellationTokenSource cancellationToken)
        {
            Logger.Info("Creating TCL interpreter.");

            Result result = null;
            var interpreter = new TclVoiceInterpreter(Eagle._Components.Public.Interpreter.Create(null, ref result));
            
            interpreter.StartAsync(cancellationToken);

            Logger.Info("TCL interpreter successfully initialized.");

            return interpreter;
        }

        #endregion

        #region Properties

        public bool IsBreakpointHit
        {
            get
            {
                lock (_lockRoot)
                {
                    return _isBreakpointHit;
                }

            }
            private set
            {
                lock (_lockRoot)
                {
                    _isBreakpointHit = value;
                }
                RaiseDebugModeChangedEvent(value);
            }
        }

        #endregion

        public void Add(ICommand command)
        {
            Logger.Debug($"Adding {command} to command stack.");
            _commandStack.Push(command);
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
                Logger.Error($"Evaluation result: {result}");
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
                    if (_commandStack.TryPop(out command))
                    {
                        command.Evaluate(this);
                    }

                    await Task.Delay(_commandEvalTimeout);
                }
            });
        }

        private ReturnCode InteractiveLoopCallback(Eagle._Components.Public.Interpreter interpreter, InteractiveLoopData loopData, ref Result result)
        {
            if(loopData.BreakpointType == BreakpointType.Demand)
            {
                //breakpoint hit, stop here
                IsBreakpointHit = true;

                Logger.Info("Stopping at breakpoint.");
                
                while(IsBreakpointHit)
                {
                    Logger.Trace($"Sleeping on breakpoint for {_commandEvalTimeout.TotalMilliseconds} milliseconds.");
                    Thread.Sleep(_commandEvalTimeout);
                }

                Logger.Info("Continuing from breakpoint.");
            }
            //continue evaluation, some other callback hit
            return ReturnCode.Ok;
        }

        private void RaiseDebugModeChangedEvent(bool isBreakpointHit)
        {
            DebugModeChanged?.Invoke(this, new DebugModeEventArgs(isBreakpointHit));
        }

        public event EventHandler<DebugModeEventArgs> DebugModeChanged;
    }
}
