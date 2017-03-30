using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Model.Infotag
{
    public class InfotagScope
    {
        public InfotagScope(InfotagScopeType scopeType)
        {
            ScopeType = scopeType;
        }

        public InfotagScopeType ScopeType { get; private set; }

    }
}
