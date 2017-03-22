using System;
using System.Collections.Generic;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Model.Infotag;
using ISubCommand = IptSimulator.CiscoTcl.Commands.Abstractions.ISubCommand;

namespace IptSimulator.CiscoTcl.Commands.Infotag
{
    public class InfotagSet: ISubCommand
    {
        private readonly IDictionary<string, IInfotagData> _infotagData;

        public InfotagSet(IDictionary<string, IInfotagData> infotagData)
        {
            _infotagData = infotagData;
        }

        public string Name => "set";

        public ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            return ReturnCode.Ok;
        }
    }
}
