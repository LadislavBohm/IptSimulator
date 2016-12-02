using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Interpreter
{
    public interface ICommand
    {
        void Evaluate(TclVoiceInterpreter interpreter);
    }
}
