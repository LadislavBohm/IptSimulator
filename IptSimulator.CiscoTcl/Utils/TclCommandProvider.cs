using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using IptSimulator.CiscoTcl.Commands;
using NLog;

namespace IptSimulator.CiscoTcl.Utils
{
    public class TclCommandProvider
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<Default> GetCustomCommands()
        {
            var customCommandsAssembly = typeof(TclCommandProvider).Assembly;

            Logger.Debug($"Loading custom TCL commands from {customCommandsAssembly.FullName} assembly.");

            var customCommandTypes = customCommandsAssembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Default)) && HasEmptyConstructor(t));

            var result = new List<Default>();
            foreach (var customCommandType in customCommandTypes)
            {
                try
                {
                    Logger.Debug($"Creating instance of {customCommandType.Name} command.");
                    var commandInstance = (Default)Activator.CreateInstance(customCommandType);
                    Logger.Debug($"{customCommandType.Name} command successfully instantiated.");

                    result.Add(commandInstance);
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Error while making an instance of {customCommandType.Name} command");
                    throw;
                }
            }

            Logger.Debug($"Found {result.Count} custom commands.");
            return result;
        }

        private static bool HasEmptyConstructor(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
