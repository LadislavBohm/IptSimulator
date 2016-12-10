using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.TclInterpreter.EventArgs;

namespace IptSimulator.CiscoTcl.Commands
{
    public class Breakpoint : Default
    {
        public Breakpoint() : base(new CommandData("breakpoint", null,null,null,typeof(Breakpoint).FullName, CommandFlags.Breakpoint, null,0))
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            int lineNumber;
            if (int.TryParse(arguments[1].String, out lineNumber))
            {
                RaiseBreakpointHitEvent(lineNumber);
            }
            else
            {
                //line number not available
                result = "Line number argument is missing. Syntax should be 'breakpoint {lineNumber}'";
                return ReturnCode.Error;
            }
            
            return ReturnCode.Ok;
        }

        private void RaiseBreakpointHitEvent(int lineNumber)
        {
            BreakpointHit?.Invoke(this, new BreakpointHitEventArgs(lineNumber));
        }

        public event EventHandler<BreakpointHitEventArgs> BreakpointHit;
    }
}
