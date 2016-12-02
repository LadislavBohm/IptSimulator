using System;
using System.Collections.Generic;
using IptSimulator.CiscoTcl.Model;

namespace IptSimulator.Client.Model.FsmGraph
{
    internal interface IFsmGraphManager
    {
        FsmGraphLogic Generate(ICollection<CiscoTcl.Model.FsmTransition> transitions);

        event EventHandler GraphPropertyChanged; 
    }
}