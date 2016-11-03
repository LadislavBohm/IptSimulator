using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels
{
    [ImplementPropertyChanged]
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel(string level, string date, string logger, string message)
        {
            LogLevel = level;
            Date = date;
            Logger = logger;
            Message = message;
        }
        
        public string LogLevel { get; set; }
        public string Date { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
    }
}
