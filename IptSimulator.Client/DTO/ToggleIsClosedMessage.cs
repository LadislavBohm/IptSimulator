using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Client.DTO
{
    public class ToggleIsClosedMessage
    {
        public ToggleIsClosedMessage(Type dockViewModelType)
        {
            DockViewModelType = dockViewModelType;
        }

        public Type DockViewModelType { get; private set; }
    }
}
