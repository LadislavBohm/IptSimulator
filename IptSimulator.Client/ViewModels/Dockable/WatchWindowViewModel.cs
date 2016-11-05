using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IptSimulator.Client.ViewModels.Data;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class WatchWindowViewModel : ViewModelBase
    {
        public WatchWindowViewModel()
        {
            InitDesignVariables();
        }

        private void InitDesignVariables()
        {
            if (IsInDesignMode)
            {
                Variables = new ObservableCollection<WatchVariableViewModel>
                {
                    new WatchVariableViewModel("version", "1.1.0"),
                    new WatchVariableViewModel("fsm", "fsm(test,enco)"),
                    new WatchVariableViewModel("currentState", "CALL_INIT")
                };
            }
            else
            {
                Variables = new ObservableCollection<WatchVariableViewModel>
                {
                    new WatchVariableViewModel("version", "1.1.0"),
                    new WatchVariableViewModel("fsm", "fsm(test,enco)"),
                    new WatchVariableViewModel("currentState", "CALL_INIT")
                };
            }
        }

        public ObservableCollection<WatchVariableViewModel> Variables { get; set; }
    }
}
