using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Data
{
    [ImplementPropertyChanged]
    public class WatchVariableViewModel : ViewModelBase
    {
        public WatchVariableViewModel(string name, string value)
        {
            Name = name;
            Value = value;
            HasSubValues = false;
        }

        public WatchVariableViewModel(string name, string value, IEnumerable<WatchVariableViewModel> subValues)
        {
            Name = name;
            Value = value;
            SubValues = subValues;
            HasSubValues = true;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public IEnumerable<WatchVariableViewModel> SubValues { get; set; }
        public bool HasSubValues { get; set; }
    }
}
