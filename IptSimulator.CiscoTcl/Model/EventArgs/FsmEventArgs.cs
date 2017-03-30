using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Model.EventArgs
{
    public class FsmEventArgs : System.EventArgs
    {
        public FsmEventArgs(string currentState, HashSet<FsmTransition> transitions)
        {
            CurrentState = currentState;
            Transitions = transitions;
        }

        public string CurrentState { get; private set; }
        public HashSet<FsmTransition> Transitions { get; private set; }
    }
}
