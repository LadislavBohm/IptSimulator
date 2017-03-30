using NLog;

namespace IptSimulator.CiscoTcl.TclInterpreter
{
    public abstract class InterpreterCommandBase : ICommand
    {
        protected static ILogger Logger = LogManager.GetCurrentClassLogger();

        public void Evaluate(TclVoiceInterpreter interpreter)
        {
            var commandName = GetType().Name;
            Logger.Info($"Evaluating {commandName} command.");
            EvaluateInternal(interpreter);
            Logger.Info($"{commandName} command evaluated.");
        }

        protected abstract void EvaluateInternal(TclVoiceInterpreter interpreter);
    }
}
