using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IptSimulator.Client.ViewModels.Abstractions;

namespace IptSimulator.Client.DTO
{
    public class CloseAllButThisMessage
    {
        public CloseAllButThisMessage(DockWindowViewModel notThis)
        {
            NotThis = notThis;
        }

        public DockWindowViewModel NotThis { get; private set; }
    }
}
