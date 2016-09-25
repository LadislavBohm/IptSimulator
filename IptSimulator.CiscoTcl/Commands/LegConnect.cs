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
    /// The leg connect command sends a signaling level CONNECT message to the incoming call leg.
    /// </summary>
    public class LegConnect : CiscoTclCommand
    {
        public LegConnect() : base(new CommandData("leg connect", null, null, null, typeof(LegConnect).FullName,CommandFlags.None, null,0))
        {
        }

        public LegConnect(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count != 3))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "legID");

                return ReturnCode.Error;
            }

            result = $"Connecting call with leg ID {arguments[2]}";
            return ReturnCode.Ok;
        }
    }
}
