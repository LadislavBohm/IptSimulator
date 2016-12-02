using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Interpreter.Commands
{
    public class EvaluteScriptCommand : InterpreterCommandBase
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
