using System;
using System.Collections.Generic;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Model.Infotag;
using NLog;
using ISubCommand = IptSimulator.CiscoTcl.Commands.Abstractions.ISubCommand;

namespace IptSimulator.CiscoTcl.Commands.Infotag
{
    public class InfotagGet: ISubCommand
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDictionary<string, IInfotagData> _infotagData;

        public InfotagGet(IDictionary<string, IInfotagData> infotagData)
        {
            _infotagData = infotagData;
        }

        public string Name => "get";

        public ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            var infotagIdentifier = arguments[0];
            _logger.Info($"Execution infotag get {infotagIdentifier} command.");


            InfotagIdentifier infotag;
            if (!InfotagProvider.All.TryGetValue(infotagIdentifier, out infotag))
            {
                _logger.Error($"Infotag {infotagIdentifier} does not exist or is not implemented.");
                result = $"Infotag {infotagIdentifier} does not exist or is not implemented.";
                return ReturnCode.Error;
            }

            if (!IsValidUse(infotag, ref result))
            {
                return ReturnCode.Error;
            }

            IInfotagData data;
            if (!_infotagData.TryGetValue(infotagIdentifier, out data))
            {
                _logger.Warn($"Infotag does not contain any data of type {infotagIdentifier}.");
                result = string.Empty;
                return ReturnCode.Ok;
            }

            _logger.Info($"Successfully get infotag {infotagIdentifier} data: {data.ToTclValue()}");
            result = data.ToTclValue();
            return ReturnCode.Ok;
        }

        private bool IsValidUse(InfotagIdentifier infotag, ref Result result)
        {
            //TODO: implement infotag validation based on its Scope
            return true;
        }
    }
}
