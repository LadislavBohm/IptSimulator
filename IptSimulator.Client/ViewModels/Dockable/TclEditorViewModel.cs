using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Model.EventArgs;
using IptSimulator.CiscoTcl.Model.InputData;
using IptSimulator.CiscoTcl.TclInterpreter;
using IptSimulator.CiscoTcl.TclInterpreter.EventArgs;
using IptSimulator.CiscoTcl.Utils;
using IptSimulator.Client.Model;
using IptSimulator.Client.ViewModels.Abstractions;
using IptSimulator.Client.ViewModels.Data;
using IptSimulator.Client.ViewModels.InputDialogs;
using IptSimulator.Core.Utils;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class TclEditorViewModel : DockWindowViewModel, IDisposable
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
        private RelayCommand _continueEvaluationCommand;
        private int? _currentBreakpointLine;
        private IEnumerable<int> _breakpoints;

        private readonly DigitInputViewModel _digitInputViewModel = new DigitInputViewModel();
        private bool _isDisposed;
        private RelayCommand _stepIntoCommand;
        private bool _delayedExecutionEnabled = true;
        private bool _pausedOnBreakpoint;
        private int _executionDelay = 100;

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

        public int? CurrentBreakpointLine
        {
            get { return _currentBreakpointLine; }
            set
            {
                _currentBreakpointLine = value;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EvaluateCommand.RaiseCanExecuteChanged();
                    EvaluateSelectionCommand.RaiseCanExecuteChanged();
                    ContinueEvaluationCommand.RaiseCanExecuteChanged();
                    StepIntoCommand.RaiseCanExecuteChanged();
                });
            }
        }

        public ObservableCollection<WatchVariableViewModel> Variables { get; set; }

        public ObservableCollection<string> Events { get; set; }

        public IEnumerable<int> Breakpoints
        {
            get { return _breakpoints; }
            set
            {
                _breakpoints = value;
                ResetBreakpoints();
                RaisePropertyChanged();
            }
        }

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

        public FsmGraphViewModel FsmGraph { get; private set; }

        public bool DelayedExecutionEnabled
        {
            get { return _delayedExecutionEnabled; }
            set
            {
                if (value == _delayedExecutionEnabled) return;
                _delayedExecutionEnabled = value; 
                RaisePropertyChanged();
                if (_delayedExecutionEnabled)
                {
                    _interpreter.EnableDelayedExecution(TimeSpan.FromMilliseconds(ExecutionDelay));
                }
                else
                {
                    _interpreter.DisableDelayedExecution();
                }
            }
        }

        public bool PausedOnBreakpoint
        {
            get { return _pausedOnBreakpoint; }
            set
            {
                _pausedOnBreakpoint = value;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EvaluateCommand.RaiseCanExecuteChanged();
                    EvaluateSelectionCommand.RaiseCanExecuteChanged();
                    ContinueEvaluationCommand.RaiseCanExecuteChanged();
                    StepIntoCommand.RaiseCanExecuteChanged();
                });
            }
        }

        public int ExecutionDelay
        {
            get { return _executionDelay; }
            set
            {
                if (value < 0 || value == _executionDelay) return;
                _executionDelay = value;
                RaisePropertyChanged();
                _interpreter.EnableDelayedExecution(TimeSpan.FromMilliseconds(value));
            }
        }

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
                               var script = InsertBreakpoints(Script);
                               if (Breakpoints != null)
                               {
                                   _logger.Debug("Inserting breakpoints to script");

                                   var breakpoints = new HashSet<int>(Breakpoints ?? new List<int>());
                                   _logger.Debug("Evaluating whole script");
                                   _interpreter.Evaluate(script, breakpoints);
                               }
                               else
                               {
                                   _logger.Debug("Evaluating whole script");
                                   _interpreter.Evaluate(script);
                               }
                            }
                           catch (Exception e)
                           {
                               _logger.Error(e, "Script evaluation has thrown an exception.");
                               EvaluationResult = string.Empty;
                           }
                       }, () => !PausedOnBreakpoint));
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
                               _logger.Debug("Inserting breakpoints to script");

                               var breakpoints = new HashSet<int>(Breakpoints ?? new List<int>());
                               var script = InsertBreakpoints(SelectedScript);

                               _logger.Debug($"Evaluating selection: {SelectedScript}");

                               _interpreter.Evaluate(script, breakpoints);
                           }
                           catch (Exception e)
                           {
                               _logger.Error(e, "Script evaluation has thrown an exception.");
                               EvaluationResult = string.Empty;
                           }
                       }, () => !PausedOnBreakpoint));
            }
        }

        public RelayCommand ContinueEvaluationCommand
        {
            get
            {
                return _continueEvaluationCommand ?? (_continueEvaluationCommand = new RelayCommand(() =>
                       {
                           _logger.Info($"Continuing evaluation from breakpoint at line {CurrentBreakpointLine}");
                           _interpreter.Continue();
                       }, () => PausedOnBreakpoint));
            }
        }

        public RelayCommand StepIntoCommand
        {
            get
            {
                return _stepIntoCommand ?? (_stepIntoCommand = new RelayCommand(() =>
                {
                    _logger.Info($"Stepping into evaluation from breakpoint at line {CurrentBreakpointLine}");
                    _interpreter.StepInto();
                }, () => PausedOnBreakpoint));
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

                        _interpreter.Evaluate($"fsm raise {SelectedEvent}");
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
            Configuration = new ConfigurationViewModel();
            FsmGraph = new FsmGraphViewModel();

            EvaluationResult = string.Empty;
            SetupEvents();

            _interpreter = TclVoiceInterpreter.Create(_cancellationTokenSource);
            if (DelayedExecutionEnabled)
            {
                _interpreter.EnableDelayedExecution(TimeSpan.FromMilliseconds(ExecutionDelay));
            }
            _interpreter.EvaluateCompleted += InterpreterOnEvaluateCompleted;
            _interpreter.WatchVariablesChanged += (sender, args) => Variables =  
                new ObservableCollection<WatchVariableViewModel>(_interpreter.WatchVariables.Select(Mapper.Map));
            _interpreter.BreakpointHitChanged += OnBreakpointHitChanged;
            _interpreter.OnInputDigitsRequested += OnInputDigitsRequested;
            _interpreter.FsmStateChanged += (sender, args) => 
                Application.Current.Dispatcher.Invoke(() => FsmGraph.SetNewState(args.CurrentState));
            _interpreter.FsmGenerated += (sender, args) => 
                Application.Current.Dispatcher.Invoke(() => FsmGraph.ResetFsmGraph(args.CurrentState, args.Transitions));
        }

        private void InterpreterOnEvaluateCompleted(object sender, EvaluteResultEventArgs evaluteResultEventArgs)
        {
            EvaluationResult = evaluteResultEventArgs.Result;
            CurrentBreakpointLine = null;
        }

        private void SetupEvents()
        {
            _allEvents = new List<string>(CiscoTclEvents.All.OrderBy(e => e));
            Events = new ObservableCollection<string>(_allEvents);
        }

        private void OnBreakpointHitChanged(object sender, DebugModeEventArgs e)
        {
            if (e.LineNumber.HasValue)
            {
                CurrentBreakpointLine = e.LineNumber.Value;
            }
            else
            {
                CurrentBreakpointLine = null;
            }
            PausedOnBreakpoint = e.IsBreakpointHit;
        }

        private void OnInputDigitsRequested(object sender, InputEventArgs<DigitsInputData> inputEventArgs)
        {
            _logger.Info("Request for input digits from interpreter received.");
            
            inputEventArgs.SetInputData(new DigitsInputData(_digitInputViewModel.GetDigits(null)));
        }

        private void ResetBreakpoints()
        {
            //don't replace breakpoints when we are not waiting at any breakpoint line
            if (CurrentBreakpointLine.HasValue)
            {
                _interpreter.ReplaceBreakpoints(Breakpoints);
            }
        }

        private void FilterEvents()
        {
            Events = string.IsNullOrWhiteSpace(FilterEventsText) ?
                new ObservableCollection<string>(_allEvents) :
                new ObservableCollection<string>(_allEvents.Where(e => e.ToUpper().Contains(FilterEventsText.ToUpper())));
        }

        private string InsertBreakpoints(string text)
        {
            var sb = new StringBuilder(text.Length * 2);
            int lineNumber = 1;
            foreach (string line in new LineReader(() => new StringReader(text)))
            {
                if (!String.IsNullOrWhiteSpace(line) && !TclUtils.IsCommentLine(line))
                {
                    sb.AppendLine(CreateBeakpointText(lineNumber));
                }
                sb.AppendLine(line);
                lineNumber++;
            }

            return sb.ToString();
        }

        private string CreateBeakpointText(int lineNumber)
        {
            return "breakpoint " + lineNumber + Environment.NewLine;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _interpreter?.Dispose();
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _isDisposed = true;
            }
        }
    }
}
