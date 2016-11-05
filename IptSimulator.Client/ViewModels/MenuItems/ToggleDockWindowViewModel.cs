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
        private readonly DockWindowViewModel _dockWindow;
        private RelayCommand _executeCommand;

        public ToggleDockWindowViewModel(string header, DockWindowViewModel dockWindow) : base(header)
        {
            _dockWindow = dockWindow;
            _dockWindow.IsClosedChanged += (sender, isClosed) => IsChecked = !isClosed;
        }

        public override RelayCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ?? (_executeCommand = new RelayCommand(() =>
                       {
                           _logger.Info($"Toggling visibility of {_dockWindow.Title} dock window.");

                           _dockWindow.IsClosed = !_dockWindow.IsClosed;
                       }, () => _dockWindow.CanClose));
            }
        }
    }
}
