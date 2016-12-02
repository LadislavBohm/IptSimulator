using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace IptSimulator.CiscoTcl.Interpreter
{
    public abstract class InterpreterCommandBase : ICommand
    {
        protected static ILogger Logger = LogManager.GetCurrentClassLogger();

        public void Evaluate(TclVoiceInterpreter interpreter)
        {
            Logger.Info($"Evaluating {GetType().Name} command.");
            EvaluateInternal(interpreter);
            Logger.Info("Command evaluated.");
        }

        protected abstract void EvaluateInternal(TclVoiceInterpreter interpreter);
    }
}
