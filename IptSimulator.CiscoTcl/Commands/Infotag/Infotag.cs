using System.Collections.Generic;
using System.Linq;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using IptSimulator.CiscoTcl.Model.Infotag;

namespace IptSimulator.CiscoTcl.Commands.Infotag
{
    public class Infotag: CiscoTclCommand
    {
        private static readonly Dictionary<string, IInfotagData> InfoTagDataInternal = new Dictionary<string, IInfotagData>();
        private readonly InfotagSet _infotagSet = new InfotagSet(InfoTagDataInternal);
        private readonly InfotagGet _infotagGet = new InfotagGet(InfoTagDataInternal);

        public Infotag() : base(new CommandData("infotag", null, null, null, typeof(Infotag).FullName, CommandFlags.None, null, 0))
        {
            TclSubCommands.Add(_infotagGet);
            TclSubCommands.Add(_infotagSet);
        }

        public static IReadOnlyDictionary<string, IInfotagData> InfoTagData => InfoTagDataInternal;

        protected override ReturnCode ExecuteInternal(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            InternalLogger.Info("Executing Infotag command.");
            InternalLogger.Debug($"Parameters: {string.Join(", ", arguments.Select(a => $"{a.Name}: {a.Value}"))}");

            if (arguments.Count < 2)
            {
                result = Utility.WrongNumberOfArguments(this, 2, arguments, string.Empty);
                ResultLogger.Error($"Incorrect number of arguments: {arguments.Count} for command infotag.");
                return ReturnCode.Error;
            }

            var subCommand = arguments[0];
            if (subCommand == _infotagGet.Name)
            {
                return _infotagGet.Execute(interpreter, clientData, arguments, ref result);
            }
            if (subCommand == _infotagSet.Name)
            {
                return _infotagSet.Execute(interpreter, clientData, arguments, ref result);
            }

            ErrorLogger.Error($"Incorrect subcommand argument: {subCommand}");
            result = $"Incorrect subcommand argument: {subCommand}";
            return ReturnCode.Error;
        }
    }
}
