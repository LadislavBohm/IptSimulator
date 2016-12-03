namespace IptSimulator.CiscoTcl.TclInterpreter
{
    public interface ICommand
    {
        void Evaluate(TclVoiceInterpreter interpreter);
    }
}
