using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Interfaces.Public;
using NLog;

namespace IptSimulator.CiscoTcl.Commands.Abstractions
{
    public abstract class CiscoTclCommand : Default
    {
        protected readonly ILogger BaseLogger = LogManager.GetCurrentClassLogger();

        protected CiscoTclCommand(ICommandData commandData) : base(commandData)
        {
        }
    }
}
