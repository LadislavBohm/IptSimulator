using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using IptSimulator.Client.DTO;
using IptSimulator.Client.ViewModels.Abstractions;
using NLog;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels
{
    [ImplementPropertyChanged]
    public class DockManagerViewModel : ViewModelBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IList<DockWindowViewModel> _allDockWindows = new List<DockWindowViewModel>();
        
        public ObservableCollection<DockWindowViewModel> Documents { get; set; }

        public ObservableCollection<object> Anchorables { get; private set; }

        public DockManagerViewModel()
        {
            this.Documents = new ObservableCollection<DockWindowViewModel>();
            this.Anchorables = new ObservableCollection<object>();

            DiscoverAndAddDockWindows();
            RegisterMessenger();
        }

        private void RegisterMessenger()
        {
            MessengerInstance.Register<ToggleIsClosedMessage>(this, m =>
            {
                _logger.Debug($"Looking for window of type {m.DockViewModelType.Name} to toggle it's IsClosed property.");
                var foundWindow = _allDockWindows.FirstOrDefault(w => w.GetType() == m.DockViewModelType);

                if (foundWindow == null)
                {
                    _logger.Warn($"Could not find window of type {m.DockViewModelType.Name}. " +
                                 $"Available windows: {string.Join(",",_allDockWindows.Select(w => w.GetType().Name))}");
                    return;
                }

                foundWindow.IsClosed = !foundWindow.IsClosed;
            });
        }

        private void DiscoverAndAddDockWindows()
        {
            _logger.Debug("Discovering all dockable windows in this assembly.");

            var dockWindows = typeof(DockManagerViewModel).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DockWindowViewModel)));

            foreach (var dockWindowType in dockWindows)
            {
                _logger.Debug($"Creating instance of {dockWindowType.Name}.");
                var dockWindowInstance = (DockWindowViewModel)Activator.CreateInstance(dockWindowType);
                dockWindowInstance.Initialize();

                _logger.Debug($"Adding {dockWindowType.Name}.");
                _allDockWindows.Add(dockWindowInstance);
                if (!dockWindowInstance.IsClosed)
                {
                    Documents.Add(dockWindowInstance);
                }
                HandleIsClosed(dockWindowInstance);
            }

            _logger.Debug($"Successfully added {_allDockWindows.Count} windows, visible windows: {Documents.Count}");
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
    }
}
