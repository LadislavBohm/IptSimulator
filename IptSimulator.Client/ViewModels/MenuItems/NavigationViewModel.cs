using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.Views;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.MenuItems
{
    [ImplementPropertyChanged]
    public class NavigationViewModel : ViewModelBase
    {
        private RelayCommand _openFsmGraphCommand;

        private Window _fsmGraphWindow;

        public RelayCommand OpenFsmGraphCommand
        {
            get
            {
                return _openFsmGraphCommand ?? (_openFsmGraphCommand = new RelayCommand(() =>
                       {
                           if (_fsmGraphWindow == null)
                           {
                               _fsmGraphWindow = new FsmVisualizationWindow();
                           }

                           var dialogResult = _fsmGraphWindow.ShowDialog();

                           _fsmGraphWindow = null;
                       }));
            }
        }
    }
}
