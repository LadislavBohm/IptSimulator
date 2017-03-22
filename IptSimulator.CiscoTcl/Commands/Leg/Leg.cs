using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;

namespace IptSimulator.CiscoTcl.Commands.Leg
{
    public class Leg : CiscoTclCommand
    {
        private readonly LegConnect _legConnect = new LegConnect();
        private readonly LegDisconnect _legDisconnect = new LegDisconnect();
        private readonly LegCollectDigits _legCollectDigits = new LegCollectDigits();

        public Leg() : base(
            new CommandData("leg", null, null, null, typeof(Leg).FullName,CommandFlags.None, null,0))
        {
            TclSubCommands.Add(_legConnect);
            TclSubCommands.Add(_legDisconnect);
            TclSubCommands.Add(_legCollectDigits);
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if (arguments == null || (arguments.Count < 3))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "leg_command command_param");

                return ReturnCode.Error;
            }

            ILegCommand legCommand = null;
            if (!ResolveLegCommand(arguments, ref legCommand, ref result))
            {
                ErrorLogger.Error(result.String);
                return ReturnCode.Error;
            }

            if (!legCommand.ValidateArguments(arguments, ref result))
            {
                ErrorLogger.Error(result);
                return ReturnCode.Error;
            }

            return legCommand.Execute(interpreter, clientData, arguments, ref result);
        }

        private bool ResolveLegCommand(ArgumentList arguments, ref ILegCommand legCommand, ref Result result)
        {
            var strCommand = arguments[1];

            if (strCommand == _legConnect.Name)
            {
                legCommand = _legConnect;
                return true;
            }

            if (strCommand == _legDisconnect.Name)
            {
                legCommand = _legDisconnect;
                return true;
            }

            if (strCommand == _legCollectDigits.Name)
            {
                legCommand = _legCollectDigits;
                return true;
            }

            legCommand = null;
            result = $"{arguments[1].String} is not a valid leg command type.";
            return false;
        }
    }
}
