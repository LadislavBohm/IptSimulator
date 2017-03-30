using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;

namespace IptSimulator.CiscoTcl.Commands.Abstractions
{
    /// <summary>
    /// Marker interface defining SubCommand of <see cref="CiscoTclCommand"/>
    /// </summary>
    public interface ISubCommand
    {
        string Name { get; }

        ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result);
    }
}
