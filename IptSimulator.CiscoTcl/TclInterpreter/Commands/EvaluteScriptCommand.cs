namespace IptSimulator.CiscoTcl.TclInterpreter.Commands
{
    internal class EvaluteScriptCommand : InterpreterCommandBase
    {
        private readonly string _scriptToEvalute;

        public EvaluteScriptCommand(string scriptToEvalute)
        {
            _scriptToEvalute = scriptToEvalute;
        }
        protected override void EvaluateInternal(TclVoiceInterpreter interpreter)
        {
            interpreter.EvaluateScript(_scriptToEvalute);
        }
    }
}
