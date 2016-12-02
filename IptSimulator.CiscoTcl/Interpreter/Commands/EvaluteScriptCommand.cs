using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Interpreter.Commands
{
    public class EvaluteScriptCommand : ICommand
    {
        private readonly string _scriptToEvalute;

        public EvaluteScriptCommand(string scriptToEvalute)
        {
            _scriptToEvalute = scriptToEvalute;
        }

        public void Evaluate()
        {
            
        }
    }
}
