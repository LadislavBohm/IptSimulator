using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.Core;

namespace IptSimulator.CiscoTcl.Commands
{
    public class FsmDefine : CiscoTclCommand
    {
        public FsmDefine() : base(
            new CommandData("fsm define", null, null, null, typeof(FsmDefine).FullName, CommandFlags.None, null, 0))
        {
        }

        public FsmDefine(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count != 4))
            {
                result = Utility.WrongNumberOfArguments(this, 2, arguments, "fsm_array initial_state");

                return ReturnCode.Error;
            }

            var fsmArray = arguments[2];
            var initialState = arguments[3];

            if (!TclUtils.ArrayExists(interpreter,fsmArray))
            {
                var arrayNotExists = $"Array of fsm states with name: {fsmArray} does not exist (is not defined).";

                BaseLogger.Error(arrayNotExists);
                result = arrayNotExists;

                return ReturnCode.Error;
            }

            //TODO: vyresit, zda validovat existenci stavu v array (projeti vsech eventu a kontrola v array?)
            //if (!TclUtils.ArrayKeyExists(interpreter,fsmArray, "CALL_INIT,ev_setup_indication"))
            //{
            //    var stateNotExists = $"State {initialState} does not exists in array {fsmArray}.";

            //    BaseLogger.Error(stateNotExists);
            //    result = stateNotExists;

            //    return ReturnCode.Error;
            //}

            BaseLogger.Info($"Setting FSM current state to {initialState}");

            if (TclUtils.SetVariable(interpreter, ref result, TclConstants.FsmCurrentStateVariable, initialState))
            {
                BaseLogger.Info($"FSM current state was successfully set to {initialState}.");
            }
            else
            {
                BaseLogger.Error($"Could not set FSM current state. Error: {result.String}");
            }

            result = $"FSM initial state {initialState} was successfully defined.";

            return ReturnCode.Ok;
        }
    }
}
