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
            HasSubValues = false;
        }

        public VariableWithValue(string variable, string value, IEnumerable<VariableWithValue> subValues)
        {
            Variable = variable;
            Value = value;
            SubValues = subValues;
            HasSubValues = subValues != null;
        }

        public string Variable { get; }
        public string Value { get; }
        public IEnumerable<VariableWithValue> SubValues { get; }
        public bool HasSubValues { get; }
    }
}
