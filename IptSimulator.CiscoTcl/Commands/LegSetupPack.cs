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
    /// The leg setupack command sends a setup acknowledgement message on the specified incoming call leg.
    /// </summary>
    public class LegSetupPack : CiscoTclCommand
    {
        public LegSetupPack() : base(new CommandData("leg setupack",null,null,null,typeof(LegSetupPack).FullName,CommandFlags.None, null,0))
        {
        }

        public LegSetupPack(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count != 3))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "legID");

                return ReturnCode.Error;
            }

            result = $"Sending setup acknowledgement to leg ID {arguments[2]}";
            return ReturnCode.Ok;
        }
    }
}
