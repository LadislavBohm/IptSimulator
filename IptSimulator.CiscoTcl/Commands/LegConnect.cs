using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using NLog;

namespace IptSimulator.CiscoTcl.Commands
{
    public class LegConnect : ILegCommand
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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

        public ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            result = "Executing log connect.";
            _logger.Info(result.String);

            return ReturnCode.Ok;
        }
    }
}
