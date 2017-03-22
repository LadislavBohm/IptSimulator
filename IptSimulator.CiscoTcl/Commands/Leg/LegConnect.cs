using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using NLog;
using ISubCommand = IptSimulator.CiscoTcl.Commands.Abstractions.ISubCommand;

namespace IptSimulator.CiscoTcl.Commands.Leg
{
    public class LegConnect : ILegCommand, ISubCommand
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public string Name => "connect";

        public bool ValidateArguments(ArgumentList arguments, ref Result result)
        {
            if (arguments.Count != 3)
            {
                var invalidNumberOfArgs = $"Invalid number of arguments. Should be 3, but is {arguments.Count}";

                _logger.Error(invalidNumberOfArgs);
                result = invalidNumberOfArgs;
                return false;
            }

            return true;
        }

        public ReturnCode Execute(Eagle._Components.Public.Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            result = "Executing log connect.";
            _logger.Info(result.String);

            return ReturnCode.Ok;
        }
    }
}
