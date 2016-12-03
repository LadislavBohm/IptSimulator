using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Model
{
    public class VariableWithValue
    {
        public VariableWithValue(string variable, string value)
        {
            Variable = variable;
            Value = value;
        }

        public string Variable { get; }
        public string Value { get; }
    }
}
