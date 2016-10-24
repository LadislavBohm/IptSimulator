using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;

namespace IptSimulator.CiscoTcl.Commands
{
    /// <summary>
    /// The leg disconnect command disconnects one or more call legs that are not part of any connection.
    /// When the command finishes, the script receives an ev_disconnect_done event.
    /// </summary>
    public class LegDisconnect : CiscoTclCommand
    {
        public LegDisconnect() : base(
            new CommandData("leg disconnect", null, null, null, typeof(LegDisconnect).FullName,CommandFlags.None, null, 0))
        {
        }

        public LegDisconnect(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count != 3))
            {
                result =  Utility.WrongNumberOfArguments(this, 1, arguments, "legID");

                return ReturnCode.Error;
            }

            result = $"Disconnecting call with leg ID {arguments[2]}";
            return ReturnCode.Ok;
        }
    }
}
