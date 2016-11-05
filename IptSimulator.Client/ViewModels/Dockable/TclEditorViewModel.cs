using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.ViewModels.Abstractions;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class TclEditorViewModel : DockWindowViewModel
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private RelayCommand _evaluateCommand;
        private RelayCommand _evaluateSelectionCommand;

        #region Properties

        public override string ContentId => "AE067FC9-714F-4EA7-926B-AD4A25B1F90A";

        public override string Title { get; set; } = "TCL editor";

        public override int Order => 1;

        public string SelectedScript { get; set; } = string.Empty;
        public string Script { get; set; }

        #endregion

        #region Commands

        public RelayCommand EvaluateCommand
        {
            get {
                return _evaluateCommand ?? (_evaluateCommand = new RelayCommand(() =>
                       {
                           _logger.Info("Evaluating whole script");
                       }));
            }
        }

        public RelayCommand EvaluateSelectionCommand
        {
            get
            {
                return _evaluateSelectionCommand ?? (_evaluateSelectionCommand = new RelayCommand(() =>
                       {
                           _logger.Info($"Evaluating selection: {SelectedScript}");
                       }));
            }
        }

        #endregion
    }
}
