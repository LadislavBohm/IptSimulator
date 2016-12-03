namespace IptSimulator.CiscoTcl.TclInterpreter.EventArgs
{
    public class DebugModeEventArgs : System.EventArgs
    {
        public DebugModeEventArgs(bool isBreakpointHit)
        {
            IsBreakpointHit = isBreakpointHit;
        }

        public bool IsBreakpointHit { get; private set; }
    }
}
