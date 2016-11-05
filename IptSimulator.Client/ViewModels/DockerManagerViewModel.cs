using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.Client.DTO;
using IptSimulator.Client.ViewModels.Abstractions;
using IptSimulator.Client.ViewModels.Dockable;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels
{
    [ImplementPropertyChanged]
    public class DockManagerViewModel : ViewModelBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IList<DockWindowViewModel> _allDockWindows = new List<DockWindowViewModel>();
        private RelayCommand _closeAllCommand;

        public DockManagerViewModel()
        {
            this.Documents = new ObservableCollection<DockWindowViewModel>();
            this.Anchorables = new ObservableCollection<object>();

            //DiscoverAndAddDockWindows();
            //SetVisibleDocuments();
            //SetActiveDocument();
            //RegisterMessenger();

            TclEditor = new TclEditorViewModel();
            DebugWindow = new DebugWindowViewModel();
        }

        #region Properties

        public ObservableCollection<DockWindowViewModel> Documents { get; set; }

        public ObservableCollection<object> Anchorables { get; private set; }

        public DockWindowViewModel ActiveDocument { get; set; }

        public DockWindowViewModel TclEditor { get; set; }
        public DockWindowViewModel DebugWindow { get; set; }

        #endregion

        public RelayCommand CloseAllCommand
        {
            get {
                return _closeAllCommand ?? (_closeAllCommand = new RelayCommand(() =>
                       {
                           foreach (var document in Documents)
                           {
                               if (document.CanClose)
                               {
                                   document.CloseCommand.Execute(null);
                               }
                           }
                       }));
                }
        }


        #region Private methods

        private void RegisterMessenger()
        {
            MessengerInstance.Register<CloseAllButThisMessage>(this, m =>
            {
                _logger.Debug($"Received request to close all windows but this one: {m.NotThis.Title}.");

                foreach (var dockWindow in _allDockWindows)
                {
                    if (dockWindow != m.NotThis && dockWindow.CanClose)
                    {
                        dockWindow.CloseCommand.Execute(this);
                    }
                }
            });
        }

        private void DiscoverAndAddDockWindows()
        {
            _logger.Debug("Discovering all dockable windows in this assembly.");

            var dockWindows = typeof(DockManagerViewModel).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DockWindowViewModel)));

            //foreach (var dockWindowType in dockWindows)
            //{
            //    _logger.Debug($"Creating instance of {dockWindowType.Name}.");
            //    var dockWindowInstance = (DockWindowViewModel)Activator.CreateInstance(dockWindowType);
            //    dockWindowInstance.Initialize();

            //    _logger.Debug($"Adding {dockWindowType.Name}.");
            //    _allDockWindows.Add(dockWindowInstance);
            //    HandleIsClosed(dockWindowInstance);
            //}

            _logger.Debug($"Successfully added {_allDockWindows.Count} windows, visible windows: {Documents.Count}");
        }

        private void SetVisibleDocuments()
        {
            foreach (var dockWindow in _allDockWindows.OrderBy(d => d.Order))
            {
                if (!dockWindow.IsClosed)
                {
                    Documents.Add(dockWindow);
                }
            }
        }

        private void SetActiveDocument()
        {
            var editorWindow = Documents.FirstOrDefault(d => d.GetType() == typeof(TclEditorViewModel));

            if (editorWindow == null)
            {
                _logger.Warn("Could not find TCL editor window in visible documents.");

                var another = Documents.FirstOrDefault();

                _logger.Info($"Setting {another} as active document.");
                ActiveDocument = another;
            }
            else
            {
                _logger.Info("Setting TCL editor as active dock window.");
                ActiveDocument = editorWindow;
            }
        }

        private void HandleIsClosed(DockWindowViewModel dockWindow)
        {
            dockWindow.IsClosedChanged += (sender, isClosed) =>
            {
                var changedDocument = sender as DockWindowViewModel;
                if (changedDocument == null) return;

                _logger.Debug($"IsClosed changed to {isClosed} on {changedDocument.Title}.");
                if (isClosed && Documents.Contains(changedDocument))
                {
                    _logger.Debug($"Removing {changedDocument.Title} from visible windows.");
                    Documents.Remove(changedDocument);
                }
                else if(!isClosed && !Documents.Contains(changedDocument))
                {
                    _logger.Debug($"Adding {changedDocument.Title} to visible windows.");
                    Documents.Add(changedDocument);
                }
            };
        }

        #endregion
    }
}
