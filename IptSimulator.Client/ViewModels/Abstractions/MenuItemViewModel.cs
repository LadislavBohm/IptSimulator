using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Abstractions
{
    [ImplementPropertyChanged]
    public abstract class MenuItemViewModel : ViewModelBase
    {
        protected MenuItemViewModel(string header)
        {
            Header = header;
        }

        #region Properties

        public string Header { get; set; }
        public bool IsChecked { get; set; }
        public bool IsCheckable { get; set; }

        #endregion

        public abstract RelayCommand ExecuteCommand { get; }
    }
}
