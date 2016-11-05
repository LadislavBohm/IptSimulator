using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IptSimulator.Client.ViewModels.Abstractions;
using IptSimulator.Client.ViewModels.Data;
using IptSimulator.Client.ViewModels.MenuItems;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels
{
    [ImplementPropertyChanged]
    public class MenuViewModel : ViewModelBase
    {

        public MenuViewModel(IEnumerable<DockWindowViewModel> dockWindows)
        {
            SetFileItems();
            SetViewItems(dockWindows);
            SetHelpItems();
        }

        #region Properties

        public ObservableCollection<MenuItemViewModel> FileItems { get; set; }
        public ObservableCollection<MenuItemViewModel> ViewItems { get; set; }
        public ObservableCollection<MenuItemViewModel> HelpItems { get; set; }

        #endregion

        private void SetHelpItems()
        {
            HelpItems = new ObservableCollection<MenuItemViewModel>();
        }

        private void SetViewItems(IEnumerable<DockWindowViewModel> windows)
        {
            ViewItems = new ObservableCollection<MenuItemViewModel>();
            foreach (var window in windows)
            {
                ViewItems.Add(new ToggleDockWindowViewModel(window.Title, window)
                {
                    IsCheckable = window.CanClose,
                    IsChecked = !window.IsClosed
                });
            }
        }

        private void SetFileItems()
        {
            FileItems = new ObservableCollection<MenuItemViewModel>();
            
        }
    }
}
