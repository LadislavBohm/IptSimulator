namespace IptSimulator.CiscoTcl.TclInterpreter.EventArgs
{
    public class DebugModeEventArgs : System.EventArgs
    {
        public DebugModeEventArgs(bool isBreakpointHit, bool isDelayedLineHit, int? lineNumber = null)
        {
            IsBreakpointHit = isBreakpointHit;
            IsDelayedLineHit = isDelayedLineHit;
            LineNumber = lineNumber;
        }

        public bool IsBreakpointHit { get; private set; }
        public int? LineNumber { get; set; }
        public bool IsDelayedLineHit { get; private set; }
    }
}
