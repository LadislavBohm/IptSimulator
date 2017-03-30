using IptSimulator.CiscoTcl.Model.InputData;

namespace IptSimulator.CiscoTcl.Model.EventArgs
{
    public class InputEventArgs<TData> : System.EventArgs where TData: BaseInputData
    {
        public TData Data { get; private set; }

        public void SetInputData(TData data)
        {
            Data = data;
        }
    }
}
