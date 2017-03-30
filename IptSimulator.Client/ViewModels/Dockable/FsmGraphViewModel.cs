using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IptSimulator.CiscoTcl.Model;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class FsmGraphViewModel : ViewModelBase
    {
        #region Properties

        public string CurrentState { get; set; }
        public HashSet<FsmTransition> Transitions { get; set; }
        
        #endregion

        #region Public methods

        public void SetNewState(string newState)
        {
            CurrentState = newState;
        }

        public void ResetFsmGraph(string initialState, HashSet<FsmTransition> transitions)
        {
            CurrentState = initialState;
            Transitions = transitions;
        }

        #endregion
    }
}
