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
    public class Leg : CiscoTclCommand
    {
        private const string LegCollectDigits = "collectdigits";
        private const string LegConnect = "connect";
        private const string LegDisconnect = "disconnect";

        public Leg() : base(
            new CommandData("leg", null, null, null, typeof(Leg).FullName,CommandFlags.None, null,0))
        {
        }

        public Leg(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count < 3))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "leg_command command_param");

                return ReturnCode.Error;
            }

            ILegCommand legCommand = null;
            if (!ResolveLegCommand(arguments, ref legCommand, ref result))
            {
                BaseLogger.Error(result.String);
                return ReturnCode.Error;
            }

            if (!legCommand.ValidateArguments(arguments, ref result))
            {
                BaseLogger.Error(result);
                return ReturnCode.Error;
            }

            return legCommand.Execute(interpreter, clientData, arguments, ref result);
        }

        private bool ResolveLegCommand(ArgumentList arguments, ref ILegCommand legCommand, ref Result result)
        {
            var strCommand = arguments[1];

            if (strCommand == LegConnect)
            {
                legCommand = new LegConnect();
                return true;
            }

            if (strCommand == LegDisconnect)
            {
                legCommand = new LegDisconnect();
                return true;
            }

            if (strCommand == LegCollectDigits)
            {
                legCommand = new LegCollectDigits();
                return true;
            }

            legCommand = null;
            result = $"{arguments[1].String} is not a valid leg command type.";
            return false;
        }
    }
}
