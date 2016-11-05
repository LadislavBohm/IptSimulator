using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eagle._Components.Public;
using GalaSoft.MvvmLight.Command;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Utils;
using IptSimulator.Client.ViewModels.Abstractions;
using IptSimulator.Client.ViewModels.Data;
using IptSimulator.Core.Tcl;
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

        private Interpreter _interpreter;
        private RelayCommand _reinitializeCommand;

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

        #endregion

        #region Commands

        public RelayCommand EvaluateCommand
        {
            get {
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

                               Result result = null;
                               var code = _interpreter.EvaluateScript(Script, ref result);
                               RefreshCurrentVariables();

                               if (code == ReturnCode.Ok)
                               {
                                   _logger.Info("Script successfully evaluated with " + 
                                       (string.IsNullOrWhiteSpace(result.String) ? "no result" : $"following result: {result}"));
                               }
                               else
                               {
                                   _logger.Warn($"Script evaluated following code: {code}. Result: {result}");
                               }
                           }
                           catch (Exception e)
                           {
                               _logger.Error(e, "Script evaluation has thrown an exception.");
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

                               Result result = null;
                               var code = _interpreter.EvaluateScript(SelectedScript, ref result);
                               RefreshCurrentVariables();

                               if (code == ReturnCode.Ok)
                               {
                                   _logger.Info($"Selected script successfully evaluated with following result: {result}");
                               }
                               else
                               {
                                   _logger.Warn($"Selected script evaluated following code: {code}. Result: {result}");
                               }
                           }
                           catch (Exception e)
                           {
                               _logger.Error(e, "Script evaluation has thrown an exception.");
                           }
                       }));
            }
        }

        public RelayCommand ReinitializeCommand
        {
            get {
                return _reinitializeCommand ?? (_reinitializeCommand = new RelayCommand(() =>
                       {
                           _logger.Info("Reinitializing interpreter.");
                           Initialize();
                       }));
                }
        }

        #endregion

        protected sealed override void Initialize()
        {
            base.Initialize();
            Variables = new ObservableCollection<WatchVariableViewModel>();

            _logger.Info("Initializing TCL Interpreter.");

            Result result = null;
            _interpreter = Interpreter.Create(null, CreateFlags.None, ref result);
            _logger.Debug("TCL Interpreter created.");

            var customCommands = TclCommandProvider.GetCustomCommands();
            _logger.Info("Adding custom TCL commands.");
            foreach (var customCommand in customCommands)
            {
                long token = 0;
                _logger.Debug($"Adding {customCommand.Name} command.");
                var code = _interpreter.AddCommand(customCommand, null, ref token, ref result);

                if (code != ReturnCode.Ok)
                {
                    _logger.Warn($"Failed to add {customCommand.Name} command. Error: {result}");
                }
            }
        }


        private void RefreshCurrentVariables()
        {
            try
            {
                _logger.Debug("Refreshing current variables and their values.");
                Result infoResult = null;
                var code = _interpreter.EvaluateScript("info vars", ref infoResult);

                var variables = infoResult.String
                    .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(v => !TclReservedVariables.All.Contains(v));

                var result = new List<VariableWithValue>();
                foreach (var variable in variables)
                {
                    Result tempResult = null;
                    if (TclUtils.GetVariableValue(_interpreter, ref tempResult, variable, true))
                    {
                        if (tempResult != null && 
                            !string.IsNullOrWhiteSpace(variable) && 
                            !string.IsNullOrWhiteSpace(tempResult.String))
                        {
                            result.Add(new VariableWithValue(variable, tempResult.String));
                        }
                    }
                    else if (TclUtils.GetVariableValue(_interpreter, ref tempResult, variable, false))
                    {
                        if (tempResult != null &&
                            !string.IsNullOrWhiteSpace(variable) &&
                            !string.IsNullOrWhiteSpace(tempResult.String))
                        {
                            result.Add(new VariableWithValue(variable, tempResult.String));
                        }
                    }
                }

                Variables.Clear();
                foreach (var variableWithValue in result)
                {
                    Variables.Add(new WatchVariableViewModel(variableWithValue.Variable,variableWithValue.Value));
                }

                
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while refreshing current variables.");
            }
        }
    }
}
