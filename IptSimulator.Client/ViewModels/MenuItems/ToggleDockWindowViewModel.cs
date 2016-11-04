using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.DTO;
using IptSimulator.Client.ViewModels.Abstractions;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.MenuItems
{
    [ImplementPropertyChanged]
    public class ToggleDockWindowViewModel : MenuItemViewModel
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Type _dockViewModelType;
        private RelayCommand _executeCommand;

        public ToggleDockWindowViewModel(string header, Type dockViewModelType) : base(header)
        {
            _dockViewModelType = dockViewModelType;
        }

        public override RelayCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ?? (_executeCommand = new RelayCommand(() =>
                       {
                           _logger.Info($"Sending toggle message for window of type {_dockViewModelType.Name}");
                           MessengerInstance.Send(new ToggleIsClosedMessage(_dockViewModelType));
                           IsChecked = !IsChecked;
                       }));
            }
        }
    }
}
