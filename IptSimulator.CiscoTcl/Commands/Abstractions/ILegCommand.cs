using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;

namespace IptSimulator.CiscoTcl.Commands.Abstractions
{
    public interface ILegCommand
    {
        bool ValidateArguments(ArgumentList arguments, ref Result result);

        ReturnCode Execute(Eagle._Components.Public.Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result);
    }
}