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
using IptSimulator.CiscoTcl.Utils;

namespace IptSimulator.CiscoTcl.Commands
{
    public class Fsm : CiscoTclCommand
    {
        private const string DefineCommand = "define";
        private const string SetStateCommand = "setstate";

        public Fsm() : base(
            new CommandData("fsm_define", null, null, null, typeof(Fsm).FullName, CommandFlags.None, null, 0))
        {
        }

        public Fsm(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if (ValidateArguments(arguments, out result) != ReturnCode.Ok) return ReturnCode.Error;

            if (arguments[1] == DefineCommand)
            {
                return ExecuteDefine(interpreter,clientData,arguments,ref result);
            }
            return ExecuteSetState(interpreter,clientData,arguments, ref result);
        }

        private ReturnCode ExecuteDefine(Interpreter interpreter, IClientData clientData, ArgumentList arguments,
            ref Result result)
        {
            var fsmArray = arguments[2];
            var initialState = arguments[3];

            if (!TclUtils.ArrayExists(interpreter, fsmArray))
            {
                var arrayNotExists = $"Array of fsm states with name: {fsmArray} does not exist (is not defined).";

                BaseLogger.Error(arrayNotExists);
                result = arrayNotExists;

                return ReturnCode.Error;
            }

            //TODO: vyresit, zda validovat existenci stavu v array (projeti vsech eventu a kontrola v array?)
            if (!FsmUtils.ContainsState(interpreter, ref result, fsmArray, initialState))
            {
                var stateNotExists = $"State {initialState} does not exists in FSM array {fsmArray}.";

                BaseLogger.Error(stateNotExists);
                result = stateNotExists;
                return ReturnCode.Error;
            }

            BaseLogger.Debug($"Settings FSM state array variable name to {fsmArray}.");
            if (TclUtils.SetVariable(interpreter, ref result, TclConstants.FsmStatesArrayNameVariable, fsmArray))
            {
                BaseLogger.Debug($"FSM state array variable name was successfully set to {fsmArray}.");
            }
            else
            {
                BaseLogger.Error($"Could not set FSM state array variable name. Error: {result.String}.");
                return ReturnCode.Error;
            }

            BaseLogger.Debug($"Setting FSM current state to {initialState}");
            if (TclUtils.SetVariable(interpreter, ref result, TclConstants.FsmCurrentStateVariable, initialState))
            {
                BaseLogger.Debug($"FSM current state was successfully set to {initialState}.");
            }
            else
            {
                BaseLogger.Error($"Could not set FSM current state. Error: {result.String}");
                return ReturnCode.Error;
            }

            result = $"FSM initial state {initialState} was successfully defined.";

            return ReturnCode.Ok;
        }

        private ReturnCode ExecuteSetState(Interpreter interpreter, IClientData clientData, ArgumentList arguments,
            ref Result result)
        {
            if ((arguments == null) || (arguments.Count != 3))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "fsm_state");

                return ReturnCode.Error;
            }

            var fsmState = arguments[2];

            if (!TclUtils.GetVariableValue(interpreter, ref result, TclConstants.FsmStatesArrayNameVariable))
            {
                var fsmStateArrayNotFound = $"Could not determine fsm array name. Checked for variable {TclConstants.FsmStatesArrayNameVariable}";
                BaseLogger.Error(fsmStateArrayNotFound);
                result = fsmStateArrayNotFound;

                return ReturnCode.Error;
            }

            var fsmArray = result.String;

            if (!FsmUtils.ContainsState(interpreter, ref result, fsmArray, fsmState))
            {
                var notContainsState = $"FSM state array does not contain {fsmState} state.";

                BaseLogger.Error(notContainsState);
                result = notContainsState;
                return ReturnCode.Error;
            }

            if (!TclUtils.SetVariable(interpreter, ref result, TclConstants.FsmCurrentStateVariable, fsmState))
            {
                var errorSettingState = $"Error occured while setting state {fsmState}. Error: {result.String}";

                BaseLogger.Error(errorSettingState);
                result = errorSettingState;
                return ReturnCode.Error;
            }

            result = $"FSM successfully set to state {fsmState}.";
            return ReturnCode.Ok;
        }

        private ReturnCode ValidateArguments(ArgumentList arguments, out Result result)
        {
            if (arguments == null)
            {
                result = "Incorrect arguments null.";
                return ReturnCode.Error;
            }
            if (arguments.Count < 3 || arguments.Count > 4)
            {
                result = Utility.WrongNumberOfArguments(this, 2, null, "define/setstate");
                return ReturnCode.Error;
            }
            if (arguments.Count == 3 && arguments[1] != SetStateCommand)
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "fsm_state");
                return ReturnCode.Error;
            }
            if (arguments.Count == 4 && arguments[1] != DefineCommand)
            {
                result = Utility.WrongNumberOfArguments(this, 2, arguments, "fsm_array init_state");
                return ReturnCode.Error;
            }
            if (arguments[1] != DefineCommand && arguments[1] != SetStateCommand)
            {
                result = $"Unknown FSM command {arguments[1]}, must be one of [{DefineCommand}, {SetStateCommand}]";
                return ReturnCode.Error;
            }
            result = string.Empty;
            return ReturnCode.Ok;
        }
    }
}
