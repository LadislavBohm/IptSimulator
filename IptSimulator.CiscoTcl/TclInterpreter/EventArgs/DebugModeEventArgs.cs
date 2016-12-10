namespace IptSimulator.CiscoTcl.TclInterpreter.EventArgs
{
    public class DebugModeEventArgs : System.EventArgs
    {
        public DebugModeEventArgs(bool isBreakpointHit, int? lineNumber = null)
        {
            IsBreakpointHit = isBreakpointHit;
            LineNumber = lineNumber;
        }

        public bool IsBreakpointHit { get; private set; }
        public int? LineNumber { get; set; }
    }
}
