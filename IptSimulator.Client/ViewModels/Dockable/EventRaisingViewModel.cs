using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IptSimulator.CiscoTcl.Events;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class EventRaisingViewModel : ViewModelBase
    {
        private IList<string> _allEvents;
        private RelayCommand _filterEventsCommand;
        private RelayCommand _raiseEventCommand;
        private string _selectedEvent;

        public EventRaisingViewModel()
        {
            SetupEvents();
        }

        #region Properties

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

        #endregion

        #region Commands

        public RelayCommand FilterEventsCommand => _filterEventsCommand ?? (_filterEventsCommand = new RelayCommand(FilterEvents));

        public RelayCommand RaiseEventCommand => _raiseEventCommand ?? 
            (_raiseEventCommand = new RelayCommand(() => { }, () => CanRaiseEvent));

        #endregion

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
