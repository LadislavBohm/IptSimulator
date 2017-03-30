using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.ViewModels.Abstractions;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.MenuItems
{
    [ImplementPropertyChanged]
    public class OpenScriptViewModel : MenuItemViewModel
    {
        private RelayCommand _executeCommand;

        public OpenScriptViewModel() : base("Open")
        {
            
        }

        public override RelayCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ?? (_executeCommand = new RelayCommand(() =>
                       {

                       }));
            }
        }
    }
}
