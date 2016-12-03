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
        }        

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
