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
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Utils;

namespace IptSimulator.CiscoTcl.Commands
{
    public class Fsm : CiscoTclCommand
    {
        private const string DefineCommand = "define";
        private const string SetStateCommand = "setstate";
        private const string RaiseEventCommand = "raise";

        public Fsm() : base(
            new CommandData("fsm", null, null, null, typeof(Fsm).FullName, CommandFlags.None, null, 0))
        {
        }

        public Fsm(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments,
            ref Result result)
        {
            try
            {
                if (ValidateArguments(arguments, out result) != ReturnCode.Ok) return ReturnCode.Error;

                if (arguments[1] == DefineCommand)
                {
                    return ExecuteDefine(interpreter, clientData, arguments, ref result);
                }
                if (arguments[1] == SetStateCommand)
                {
                    return ExecuteSetState(interpreter, clientData, arguments, ref result);
                }
                if (arguments[1] == RaiseEventCommand)
                {
                    return ExecuteRaiseEvent(interpreter, clientData, arguments, ref result);
                }

                var invalidArgs = $"Invalid argument {arguments[1]} in FSM command.";

                BaseLogger.Error(invalidArgs);
                result = invalidArgs;
                return ReturnCode.Error;
            }
            catch (Exception e)
            {
                BaseLogger.Error(e,"Unexpected error occured while executing Fsm command. " +
                                   $"Arguments: {(arguments == null ? "NULL" : string.Join(",",arguments))}");
                return ReturnCode.Error;
            }
        }

        private ReturnCode ExecuteRaiseEvent(Interpreter interpreter, IClientData clientData, ArgumentList arguments,
            ref Result result)
        {
            var raisedEvent = arguments[2];

            if (!CiscoTclEvents.All.Contains(raisedEvent.String))
            {
                BaseLogger.Error($"Event {raisedEvent} is not valid Cisco TCL event.");
                BaseLogger.Debug($"Valid CiscoTcl events are: [{string.Join(", ", CiscoTclEvents.All)}]");

                result = $"Event {raisedEvent} is not valid Cisco TCL event.";
                return ReturnCode.Error;
            }

            if (!TclUtils.GetVariableValue(interpreter, ref result, TclConstants.FsmStatesArrayNameVariable))
            {
                var fsmStateArrayNotFound = $"Could not determine fsm array name. Checked for variable {TclConstants.FsmStatesArrayNameVariable}";

                BaseLogger.Error(fsmStateArrayNotFound);
                result = fsmStateArrayNotFound;
                return ReturnCode.Error;
            }

            var fsmArray = result.String;
            IEnumerable<FsmTransition> transitions;

            if (!FsmUtils.TryGetFsmTransitions(interpreter, fsmArray, out transitions))
            {
                var transitionsFailed = $"Failed to get FSM transitions. Fsm array: {fsmArray}";

                BaseLogger.Error(transitionsFailed);
                result = transitionsFailed;
                return ReturnCode.Error;
            }

            var transition = transitions.FirstOrDefault(t => t.Event == raisedEvent);

            if (transition == null)
            {
                var transitionNotDefined = $"Transition for event {raisedEvent} is not defined in FSM.";

                BaseLogger.Warn(transitionNotDefined);
                result = transitionNotDefined;
                return ReturnCode.Ok;
            }
            
            BaseLogger.Info($"Found transition for event {raisedEvent} is {transition}.");

            if (!TclUtils.ProcedureExists(interpreter, transition.Procedure))
            {
                var procedureNotExists = $"Procedure {transition.Procedure} which is defined in transition {transition} does not exist.";

                BaseLogger.Error(procedureNotExists);
                result = procedureNotExists;
                return ReturnCode.Error;
            }

            var currentStateToSet = transition.DetermineActualTargetState();

            if (!FsmUtils.ContainsState(interpreter, ref result, fsmArray, currentStateToSet))
            {
                var notDefinedState = $"Target state {currentStateToSet} defined in transition is not defined in FSM state array";

                BaseLogger.Error(notDefinedState);
                BaseLogger.Info($"Executed transition: {transition}");
                result = notDefinedState;
                return ReturnCode.Error;
            }

            var code = interpreter.EvaluateScript($"{transition.Procedure}", ref result);

            if (code != ReturnCode.Ok)
            {
                var error = $"Error occured while executing procedure {transition.Procedure}. Error: {result.String}";

                BaseLogger.Error(error);
                result = error;
                return ReturnCode.Error;
            }

            if (!TclUtils.SetVariable(interpreter, ref result, TclConstants.FsmCurrentStateVariable,currentStateToSet))
            {
                var error = $"Error while setting current FSM state after executing procedure {transition.Procedure}. State: {currentStateToSet}. Error: {result}";

                BaseLogger.Error(error);
                result = error;
                return ReturnCode.Error;
            }

            return ReturnCode.Ok;
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
            //TODO: upravit tak, aby stav vlozilo do promenne overriden state a ten potom po presunu do stavu zase smazala
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

            if (!TclUtils.SetVariable(interpreter, ref result, TclConstants.FsmOverriddenStateVariable, fsmState))
            {
                var errorSettingState = $"Error occured while setting overriden state {fsmState}. Error: {result.String}";

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
            if (arguments.Count == 3 && (arguments[1] != SetStateCommand && arguments[1] != RaiseEventCommand))
            {
                result = Utility.WrongNumberOfArguments(this, 1, arguments, "fsm_state");
                return ReturnCode.Error;
            }
            if (arguments.Count == 4 && arguments[1] != DefineCommand)
            {
                result = Utility.WrongNumberOfArguments(this, 2, arguments, "fsm_array init_state");
                return ReturnCode.Error;
            }
            if (arguments[1] != DefineCommand && arguments[1] != SetStateCommand && arguments[1] != RaiseEventCommand)
            {
                result = $"Unknown FSM command {arguments[1]}, must be one of [{DefineCommand}, {SetStateCommand}, {RaiseEventCommand}]";
                return ReturnCode.Error;
            }
            result = string.Empty;
            return ReturnCode.Ok;
        }
    }
}
