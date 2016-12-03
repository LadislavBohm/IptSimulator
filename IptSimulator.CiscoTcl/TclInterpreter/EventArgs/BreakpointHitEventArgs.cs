namespace IptSimulator.CiscoTcl.TclInterpreter.EventArgs
{
    public class BreakpointHitEventArgs : System.EventArgs
    {
        public int? LineNumber { get; private set; }

        public BreakpointHitEventArgs()
        {
            LineNumber = null;
        }

        public BreakpointHitEventArgs(int lineNumber)
        {
            LineNumber = lineNumber;
        }
    }
}
