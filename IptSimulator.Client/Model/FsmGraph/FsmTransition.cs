using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Models;

namespace IptSimulator.Client.Model.FsmGraph
{
    public class FsmTransition : EdgeBase<FsmState>
    {
        public FsmTransition(FsmState source, FsmState target, string @event, string procedure, double weight = 1) : 
            base(source, target, weight)
        {
            Event = @event;
            Procedure = procedure;
        }

        public string Event { get; private set; }
        public string Procedure { get; private set; }
    }
}
