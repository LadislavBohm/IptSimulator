using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.Model.NLog;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels
{
    [ImplementPropertyChanged]
    public class DebugWindowViewModel : ViewModelBase
    {
        private IList<LogViewModel> _allLogs = new List<LogViewModel>();
        private RelayCommand _filterLogsCommand;

        public DebugWindowViewModel()
        {
            NLogDebugWindowTarget.LogReceived += OnLogReceived;
        }

        #region Properties

        public bool Debug { get; set; } = true;
        public bool Info { get; set; } = true;
        public bool Warning { get; set; } = true;
        public bool Error { get; set; } = true;
        public string LoggerFilter { get; set; } = string.Empty;

        public ObservableCollection<LogViewModel> Logs { get; set; } = new ObservableCollection<LogViewModel>();
        
        #endregion

        #region Commands

        public RelayCommand FilterLogsCommand
        {
            get
            {
                return _filterLogsCommand ?? (_filterLogsCommand = new RelayCommand(() =>
                       {
                           Logs = new ObservableCollection<LogViewModel>(_allLogs.Where(CanAdd));
                       }));
            }
        }

        #endregion

        private void OnLogReceived(object sender, NLogWriteEventArgs e)
        {
            var log = new LogViewModel(
                e.LogEventInfo.Level.ToString().ToUpper(),
                DateTime.Now.ToString("HH:mm:ss.fff"),
                e.LogEventInfo.LoggerName.Split(new[] { "." }, StringSplitOptions.None).Last(),
                e.LogEventInfo.FormattedMessage);

            _allLogs.Add(log);
            if (CanAdd(e.LogEventInfo))
            {
                Logs.Add(log);
            }
        }

        private bool CanAdd(LogEventInfo eventInfo)
        {
            if (eventInfo.Level == LogLevel.Debug)
            {
                return Debug;
            }
            if (eventInfo.Level == LogLevel.Info)
            {
                return Info;
            }
            if (eventInfo.Level == LogLevel.Warn)
            {
                return Warning;
            }
            return Error; 
        }

        private bool CanAdd(LogViewModel log)
        {
            if (!string.IsNullOrWhiteSpace(LoggerFilter) && 
                !log.Logger.ToUpper().Contains(LoggerFilter.ToUpper()))
            {
                return false;
            }
            if (string.Equals(log.LogLevel, LogLevel.Debug.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                return Debug;
            }
            if (string.Equals(log.LogLevel, LogLevel.Info.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                return Info;
            }
            if (string.Equals(log.LogLevel, LogLevel.Warn.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                return Warning;
            }
            return Error;
        }
    }
}
