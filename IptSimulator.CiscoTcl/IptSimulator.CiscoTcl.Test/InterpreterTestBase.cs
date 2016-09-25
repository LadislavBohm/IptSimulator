using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Test
{
    public abstract class InterpreterTestBase
    {
        protected readonly ScriptDataProvider ScriptProvider = new ScriptDataProvider();
    }
}
