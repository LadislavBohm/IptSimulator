using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.Client.Controls.InputDialogs;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.InputDialogs
{
    [ImplementPropertyChanged]
    public class DigitInputViewModel : BaseInputDialogViewModel
    {
        private DigitInputDialog _digitInputDialog;
        private RelayCommand<string> _addCharacterCommand;
        private string _digitString;

        private static readonly HashSet<string> AllowedCharacters = new HashSet<string>(new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "*", "#" });
        private RelayCommand _removeCharacterCommand;
        private const string UnknownEvent = "unknown";

        public DigitInputViewModel()
        {
            DigitString = string.Empty;
            if (IsInDesignMode)
            {
                Instructions =
                    "Bacon ipsum dolor amet brisket ham ground round, jowl venison tongue beef ribs corned beef ribeye andouille turducken. Burgdoggen kevin pork loin doner kielbasa ham ribeye. Turkey chuck ground round, alcatra frankfurter porchetta pork chop corned beef beef pork loin leberkas. Boudin cow pork chop, jowl beef turkey ribeye salami flank burgdoggen filet mignon kevin leberkas doner pork loin.";
                DigitString = "737572542";
                SourceEvent = CiscoTclEvents.AuthenticateDone;
            }
        }

        public string DigitString
        {
            get { return _digitString; }
            set
            {
                if (value == null || !value.All(character => AllowedCharacters.Contains(character.ToString()))) return;
                _digitString = value;
                RaisePropertyChanged();
                AddCharacterCommand.RaiseCanExecuteChanged();
                RemoveCharacterCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<string> AddCharacterCommand
        {
            get
            {
                return _addCharacterCommand ?? (_addCharacterCommand = new RelayCommand<string>(character =>
                {
                    DigitString = DigitString + character;
                }, character =>
                     !string.IsNullOrWhiteSpace(character) &&
                     AllowedCharacters.Contains(character) &&
                     (DigitString == null || DigitString.Length < 15) &&
                     character.Length == 1
                ));
            }
        }

        public RelayCommand RemoveCharacterCommand
        {
            get
            {
                return _removeCharacterCommand ?? (_removeCharacterCommand = new RelayCommand(() =>
                {
                    DigitString = DigitString.Length == 1 ? string.Empty : DigitString.Remove(DigitString.Length - 1);
                }, () => !string.IsNullOrWhiteSpace(DigitString)));
            }
        }

        public string GetDigits(string sourceEvent)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                _digitInputDialog = new DigitInputDialog() { DataContext = this };
                SourceEvent = sourceEvent ?? UnknownEvent;
                if (_digitInputDialog.ShowDialog() == true)
                {
                    return DigitString;
                }
                return string.Empty;
            });
        }

        protected override void ProcessAfterOk()
        {
            _digitInputDialog.DialogResult = true;
            _digitInputDialog.Close();
            _digitInputDialog = null;
        }

        protected override void ProcessAfterCancel()
        {
            _digitInputDialog.DialogResult = true;
            _digitInputDialog.Close();
            _digitInputDialog = null;
        }
    }
}
