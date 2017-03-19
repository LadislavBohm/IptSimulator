using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Model.InputData;

namespace IptSimulator.CiscoTcl.Commands.Abstractions
{
    public interface IInputRequestingCommand<TData> where TData : BaseInputData
    {
        event EventHandler<InputEventArgs<TData>> OnInputRequested;
    }
}
