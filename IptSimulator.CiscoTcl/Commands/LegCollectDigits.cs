using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Model.InputData;
using NLog;
using ISubCommand = IptSimulator.CiscoTcl.Commands.Abstractions.ISubCommand;

namespace IptSimulator.CiscoTcl.Commands
{
    public sealed class LegCollectDigits : InputRequestingCommandBase<DigitsInputData>, ILegCommand, ISubCommand
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This regex validates phone number by E.164 standard
        /// Taken from: http://stackoverflow.com/questions/6478875/regular-expression-matching-e-164-formatted-phone-numbers
        /// </summary>
        private const string E164Regex = @"^\+?[1-9]\d{1,14}$";

        public bool ValidateArguments(ArgumentList arguments, ref Result result)
        {
            if (arguments.Count < 4)
            {
                var invalidNumberOfArgs = $"Invalid number of arguments. Should be 4, but is {arguments.Count}";

                _logger.Error(invalidNumberOfArgs);
                result = invalidNumberOfArgs;
                return false;
            }

            return true;
        }

        public ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            _logger.Info("Executing log collectdigits.");

            try
            {
                RequestAndProcessInputData();

                _logger.Info("Leg collectidigits successfully finished it's execution.");
                _logger.Debug($"Collected digits: {InputData}");

                result = InputData.CollectedDigits;
                return ReturnCode.Ok;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while requesting input data.");
                result = string.Empty;
                return ReturnCode.Error;
            }
        }

        protected override bool IsInputDataValid(DigitsInputData inputData)
        {
            return !string.IsNullOrWhiteSpace(inputData?.CollectedDigits) && Regex.IsMatch(inputData.CollectedDigits, E164Regex);
        }
    }
}
