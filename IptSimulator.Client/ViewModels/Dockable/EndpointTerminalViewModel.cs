using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IptSimulator.Client.Model;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class EndpointTerminalViewModel : ViewModelBase
    {

        public EndpointTerminalViewModel()
        {
            EndpointTypes = new ObservableCollection<TerminalEndpointType>((TerminalEndpointType[])Enum.GetValues(typeof(TerminalEndpointType)));
        }

        #region Properties

        public string TerminalName { get; set; }

        public ObservableCollection<TerminalEndpointType> EndpointTypes { get; private set; }

        public TerminalEndpointType SelectedType { get; set; } = TerminalEndpointType.POTS;

        #endregion
    }
}
