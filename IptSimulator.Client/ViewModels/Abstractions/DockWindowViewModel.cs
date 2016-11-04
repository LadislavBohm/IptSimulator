using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.Model.Interfaces;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Abstractions
{
    [ImplementPropertyChanged]
    public abstract class DockWindowViewModel : ViewModelBase
    {
        private RelayCommand _closeCommand;
        private RelayCommand _openCommand;
        private bool _isClosed;

        #region Properties

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
