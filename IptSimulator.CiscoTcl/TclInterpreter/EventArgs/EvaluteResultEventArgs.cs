using Eagle._Components.Public;

namespace IptSimulator.CiscoTcl.TclInterpreter.EventArgs
{
    public class EvaluteResultEventArgs : System.EventArgs
    {
        public Result Result { get; private set; }
        public ReturnCode ReturnCode { get; private set; }
        public int ErrorLine { get; private set; }

        public EvaluteResultEventArgs(Result result, ReturnCode returnCode, int errorLine)
        {
            Result = result;
            ReturnCode = returnCode;
            ErrorLine = errorLine;
        }
    }
}
