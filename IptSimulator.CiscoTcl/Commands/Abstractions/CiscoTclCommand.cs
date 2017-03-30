using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using NLog;

namespace IptSimulator.CiscoTcl.Commands.Abstractions
{
    public abstract class CiscoTclCommand : Default
    {
        protected readonly ILogger ResultLogger = LogManager.GetLogger(typeof(CiscoTclCommand).FullName + ".Result");
        protected readonly ILogger InternalLogger = LogManager.GetLogger(typeof(CiscoTclCommand).FullName);
        protected readonly ILogger ErrorLogger = LogManager.GetLogger(typeof(CiscoTclCommand).FullName + ".Error");

        public HashSet<ISubCommand> TclSubCommands { get; private set; } = new HashSet<ISubCommand>();

        protected CiscoTclCommand(ICommandData commandData) : base(commandData)
        {
        }

        public sealed override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            InternalLogger.Info($"Executing {Name} command.");

            InternalLogger.Debug($"Calling {nameof(PreExecute)} method.");
            PreExecute(interpreter, clientData, arguments);
            InternalLogger.Debug($"{nameof(PreExecute)} method finished.");

            var returnCode = ExecuteInternal(interpreter, clientData, arguments, ref result);

            InternalLogger.Debug($"Calling {nameof(PostExecute)} method.");
            PostExecute(interpreter, clientData, arguments);
            InternalLogger.Debug($"{nameof(PostExecute)} method finished.");

            InternalLogger.Info($"{Name} command finished with code {returnCode}.");
            return returnCode;
        }

        protected abstract ReturnCode ExecuteInternal(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result);

        protected virtual void PreExecute(Interpreter interpreter, IClientData clientData, ArgumentList arguments)
        {
            //place for implementations to do some processing
        }

        protected virtual void PostExecute(Interpreter interpreter, IClientData clientData, ArgumentList arguments)
        {
            //place for implementations to do some processing
        }
    }
}
