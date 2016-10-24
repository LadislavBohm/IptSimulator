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
    /// The leg collectdigits command instructs the system to collect digits on a specified call leg against a dial
    /// plan, a list of patterns, or both.
    /// </summary>
    public class LegCollectDigits : CiscoTclCommand
    {
        public LegCollectDigits() : base(
            new CommandData("leg collectdigits", null, null, null, typeof(LegCollectDigits).FullName,CommandFlags.None, null,0))
        {
        }

        public LegCollectDigits(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count < 3))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "legID");

                return ReturnCode.Error;
            }

            result = $"Collecting digits for params: {string.Join(",", arguments.Skip(2))}";
            return ReturnCode.Ok;
        }
    }
}
