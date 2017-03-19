using System;
using System.Collections.Generic;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using ISubCommand = IptSimulator.CiscoTcl.Commands.Abstractions.ISubCommand;

namespace IptSimulator.CiscoTcl.Commands.Infotag
{
    public class InfotagGet: ISubCommand
    {
        private readonly IDictionary<string, object> _infotagData;

        public InfotagGet(IDictionary<string, object> infotagData)
        {
            _infotagData = infotagData;
        }

        public string Name => "get";

        public ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            throw new NotImplementedException();
        }
    }
}
