using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.DTO;
using IptSimulator.Client.Model.Interfaces;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Abstractions
{
    [ImplementPropertyChanged]
    public abstract class DockWindowViewModel : ViewModelBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private RelayCommand _closeCommand;
        private RelayCommand _openCommand;
        private bool _isClosed;
        private RelayCommand _closeAllButThisCommand;

        #region Properties

        public abstract string ContentId { get; }

        public abstract string Title { get; set; }

        public virtual bool CanClose { get;  } = true;

        public virtual bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                _isClosed = value;
                RaisePropertyChanged();
                RaiseIsClosedChangedEvent();
            }
        }

        public virtual int Order { get; } = int.MaxValue;

        #endregion

        #region Commands

        public RelayCommand CloseCommand
        {
            get {
                return _closeCommand ?? (_closeCommand = new RelayCommand(() =>
                       {
                           if (CanClose)
                           {
                               IsClosed = true;
                               RaiseIsClosedChangedEvent();
                           }
                       },() => CanClose));
            }
        }

        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(() =>
                       {
                           IsClosed = false;
                           RaiseIsClosedChangedEvent();
                       }));
            }
        }

        public RelayCommand CloseAllButThisCommand
        {
            get
            {
                return _closeAllButThisCommand ?? (_closeAllButThisCommand = new RelayCommand(() =>
                       {
                           _logger.Debug($"Sending request to close all windows but this one: {Title}");
                           MessengerInstance.Send(new CloseAllButThisMessage(this));
                       }));
            }
        }

        #endregion

        public virtual void Initialize()
        {
            //let overrides do necessary initialization
        }

        private void RaiseIsClosedChangedEvent()
        {
            IsClosedChanged?.Invoke(this,IsClosed);
        }

        public event EventHandler<bool> IsClosedChanged; 
    }
}
