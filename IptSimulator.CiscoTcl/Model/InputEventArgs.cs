using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IptSimulator.CiscoTcl.Model.InputData;

namespace IptSimulator.CiscoTcl.Model
{
    public class InputEventArgs<TData> : EventArgs where TData: BaseInputData
    {
        public TData Data { get; private set; }

        public void SetInputData(TData data)
        {
            Data = data;
        }
    }
}
