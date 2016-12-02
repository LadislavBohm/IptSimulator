using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Interpreter
{
    public class DebugModeEventArgs : EventArgs
    {
        public DebugModeEventArgs(bool isBreakpointHit)
        {
            IsBreakpointHit = isBreakpointHit;
        }

        public bool IsBreakpointHit { get; private set; }
    }
}
