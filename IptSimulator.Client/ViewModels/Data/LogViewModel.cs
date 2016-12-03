using GalaSoft.MvvmLight;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Data
{
    [ImplementPropertyChanged]
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel(string level, string date, string logger, string message, string exception)
        {
            LogLevel = level;
            Date = date;
            Logger = logger;
            Message = message;
            Exception = exception;
        }
        
        public string LogLevel { get; set; }
        public string Date { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
