using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Model.InputData
{
    public class DigitsInputData : BaseInputData
    {
        public string CollectedDigits { get; private set; }

        public DigitsInputData(string collectedDigits)
        {
            CollectedDigits = collectedDigits;
        }
    }
}
