using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Utils
{
    public class FsmSpecialStates
    {
        public const string AnyState = "any_state";
        public const string SameState = "same_state";

        public static IReadOnlyCollection<string> All { get; } = new List<string> {AnyState, SameState};
    }
}
