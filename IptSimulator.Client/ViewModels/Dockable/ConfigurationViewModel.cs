using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class ConfigurationViewModel : ViewModelBase
    {
        #region Properties

        [DisplayName("Timer timeout")]
        [Description("Default timeout that will be used when timer command is executed.")]
        public TimeSpan TimerTimeout { get; set; } = TimeSpan.FromSeconds(4);

        #endregion
    }
}
