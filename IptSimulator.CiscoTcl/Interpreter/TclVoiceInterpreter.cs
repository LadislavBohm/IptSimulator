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

        public void Add(ICommand command)
        {
            Logger.Debug($"Adding {command} to command stack.");
            _commandStack.Push(command);
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
                        command.Evaluate();
                    }

                    await Task.Delay(_commandEvalTimeout);
                }
            });
        }

        private ReturnCode InteractiveLoopCallback(Eagle._Components.Public.Interpreter interpreter, InteractiveLoopData loopData, ref Result result)
        {

            return ReturnCode.Ok;
        }

        private void RaiseDebugModeChangedEvent(bool isBreakpointHit)
        {
            DebugModeChanged?.Invoke(this, new DebugModeEventArgs(isBreakpointHit));
        }

        public event EventHandler<DebugModeEventArgs> DebugModeChanged;
    }
}
