using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Eagle._Components.Public;
using GalaSoft.MvvmLight.Command;
using IptSimulator.CiscoTcl.Commands;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.Client.Model;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        private Result _result;
        private readonly Interpreter _interpreter;
        private RelayCommand<EventWithDescription> _raiseEventCommand;
        private RelayCommand<string> _evaluateScriptCommand;

        public MainViewModel()
        {
            _interpreter = Interpreter.Create(null, ref _result);
            long token = 0;
            _interpreter.AddCommand(new Fsm(), null, ref token, ref _result);
            Events = new ObservableCollection<EventWithDescription>(
                    CiscoTclEvents.All.Select(e => new EventWithDescription(e, e)));
        }

        public ObservableCollection<EventWithDescription> Events { get; set; }

        public EventWithDescription SelectedEvent { get; set; }

        public RelayCommand<EventWithDescription> RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??
                       (_raiseEventCommand = new RelayCommand<EventWithDescription>((selectedEvent) =>
                       {
                           if (selectedEvent == null)
                           {
                               return;
                           }

                           try
                           {
                               var code = _interpreter.EvaluateScript($"fsm raise {selectedEvent.Event}", ref _result);
                               MessageBox.Show($"RESULT CODE IS: {code}, RESULT VALUE IS: {_result.String}");
                           }
                           catch (Exception e)
                           {
                               MessageBox.Show($"EXCEPTION OCCURED: {e.Message}");
                               throw;
                           }
                       }));
            }
        }

        public RelayCommand<string> EvaluateScriptCommand
        {
            get
            {
                return _evaluateScriptCommand ?? (_evaluateScriptCommand = new RelayCommand<string>((script) =>
                       {
                           try
                           {
                               var code = _interpreter.EvaluateScript(script, ref _result);
                               MessageBox.Show($"RESULT CODE IS: {code}, RESULT VALUE IS: {_result}");
                           }
                           catch (Exception e)
                           {
                               MessageBox.Show($"EXCEPTION OCCURED: {e.Message}");
                               throw;
                           }
                }));  
            }
        }
    }
}
