using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eagle._Components.Public;
using Eagle._Components.Public.Delegates;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using GalaSoft.MvvmLight.Command;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.TclInterpreter;
using IptSimulator.CiscoTcl.Utils;
using IptSimulator.Client.ViewModels.Abstractions;
using IptSimulator.Client.ViewModels.Data;
using IptSimulator.Core.Tcl;
using Newtonsoft.Json;
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
        private RelayCommand _reinitializeCommand;
        private RelayCommand _filterEventsCommand;
        private RelayCommand _raiseEventCommand;
        private TclVoiceInterpreter _interpreter;
        private IList<string> _allEvents;
        private string _selectedEvent;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public TclEditorViewModel()
        {
            Initialize();
        }

        #region Properties

        public override string ContentId => "AE067FC9-714F-4EA7-926B-AD4A25B1F90A";

        public override string Title { get; set; } = "TCL editor";

        public override int Order => 1;

        public string SelectedScript { get; set; } = string.Empty;

        public string Script { get; set; }

        public ObservableCollection<WatchVariableViewModel> Variables { get; set; }

        public ObservableCollection<string> Events { get; set; }

        public string SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                RaisePropertyChanged();
                RaiseEventCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanRaiseEvent => !string.IsNullOrWhiteSpace(SelectedEvent);

        public string FilterEventsText { get; set; }

        public string EvaluationResult { get; set; } = string.Empty;

        public ConfigurationViewModel Configuration { get; private set; }

        #endregion

        #region Commands

        public RelayCommand EvaluateCommand
        {
            get
            {
                return _evaluateCommand ?? (_evaluateCommand = new RelayCommand(() =>
                       {
                           try
                           {
                               if (string.IsNullOrWhiteSpace(Script))
                               {
                                   _logger.Info("No script to evaluate.");
                                   return;
                               }

                               _logger.Debug("Evaluating whole script");

                               _interpreter.Evaluate(Script);
                           }
                           catch (Exception e)
                           {
                               _logger.Error(e, "Script evaluation has thrown an exception.");
                               EvaluationResult = string.Empty;
                           }
                       }));
            }
        }

        public RelayCommand EvaluateSelectionCommand
        {
            get
            {
                return _evaluateSelectionCommand ?? (_evaluateSelectionCommand = new RelayCommand(() =>
                       {
                           try
                           {
                               if (string.IsNullOrWhiteSpace(SelectedScript))
                               {
                                   _logger.Info("No selected script to evaluate.");
                                   return;
                               }

                               _logger.Debug($"Evaluating selection: {SelectedScript}");

                               _interpreter.Evaluate(SelectedScript);
                           }
                           catch (Exception e)
                           {
                               _logger.Error(e, "Script evaluation has thrown an exception.");
                               EvaluationResult = string.Empty;
                           }
                       }));
            }
        }

        public RelayCommand ReinitializeCommand
        {
            get
            {
                return _reinitializeCommand ?? (_reinitializeCommand = new RelayCommand(() =>
                       {
                           _logger.Info("Reinitializing interpreter.");
                           Initialize();
                       }));
            }
        }

        public RelayCommand FilterEventsCommand
            => _filterEventsCommand ?? (_filterEventsCommand = new RelayCommand(FilterEvents));

        public RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ?? (_raiseEventCommand = new RelayCommand(() =>
                {
                    try
                    {
                        _logger.Debug($"Raising {SelectedEvent} command.");

                        //Result result = null;
                        //var code = _interpreter.EvaluateScript($"fsm raise {SelectedEvent}", ref result);
                        //RefreshCurrentVariables();

                        //if (code == ReturnCode.Ok)
                        //{
                        //    _logger.Info($"{SelectedEvent} event was successfully raised with following result: {result}");
                        //}
                        //else
                        //{
                        //    _logger.Warn($"{SelectedEvent} event raised with following code: {code}. Result: {result}");
                        //}
                        //EvaluationResult = result;
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, $"Error while raising {SelectedEvent} event.");
                        EvaluationResult = string.Empty;
                    }
                }
            ,
            () => CanRaiseEvent));
            }
        }

        #endregion

        protected sealed override void Initialize()
        {
            base.Initialize();

            Variables = new ObservableCollection<WatchVariableViewModel>();
            EvaluationResult = string.Empty;
            SetupEvents();

            _logger.Info("Initializing TCL Interpreter.");

            _interpreter = TclVoiceInterpreter.Create(_cancellationTokenSource);
            _interpreter.EvaluateCompleted += (sender, args) =>
            {
                EvaluationResult = args.Result;
            };
            _interpreter.WatchVariablesChanged += (sender, args) =>
            {
                Variables = new ObservableCollection<WatchVariableViewModel>(
                    _interpreter.WatchVariables.Select(vw => new WatchVariableViewModel(vw.Variable, vw.Value)));
            };

            _logger.Info("Initializing configuration");
            Configuration = new ConfigurationViewModel();
        }

        private void SetupEvents()
        {
            _allEvents = new List<string>(CiscoTclEvents.All.OrderBy(e => e));
            Events = new ObservableCollection<string>(_allEvents);
        }

        private void FilterEvents()
        {
            Events = string.IsNullOrWhiteSpace(FilterEventsText) ?
                new ObservableCollection<string>(_allEvents) :
                new ObservableCollection<string>(_allEvents.Where(e => e.ToUpper().Contains(FilterEventsText.ToUpper())));
        }
    }
}
