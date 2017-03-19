using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstFloor.ModernUI.Presentation;
using GalaSoft.MvvmLight;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.InputDialogs
{
    [ImplementPropertyChanged]
    public abstract class BaseInputDialogViewModel : ViewModelBase
    {
        private RelayCommand _okCommand;
        private RelayCommand _cancelCommand;


        public string Instructions { get; set; }
        public string SourceEvent { get; set; }

        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand((a) =>
                {
                    if (IsValid())
                    {
                        ProcessAfterOk();
                        RaiseOnCompleted();
                    }
                }));
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand((a) =>
                {
                    ProcessAfterCancel();
                    RaiseOnCancel();
                }));
            }
        }

        protected virtual bool IsValid()
        {
            //base implementation doesn't validate anything
            return true;
        }

        protected abstract void ProcessAfterOk();

        protected abstract void ProcessAfterCancel();

        protected virtual void RaiseOnCancel()
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaiseOnCompleted()
        {
            OnCompleted?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnCancel;
        public event EventHandler OnCompleted;
    }
}
