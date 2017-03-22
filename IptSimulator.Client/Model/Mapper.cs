using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.Client.ViewModels.Data;

namespace IptSimulator.Client.Model
{
    internal static class Mapper
    {
        public static WatchVariableViewModel Map(VariableWithValue variableWithValue)
        {
            if (variableWithValue.HasSubValues)
            {
                return new WatchVariableViewModel(variableWithValue.Variable, variableWithValue.Value, variableWithValue.SubValues.Select(Map));
            }
            return new WatchVariableViewModel(variableWithValue.Variable, variableWithValue.Value);
        }
    }
}
