using System;
using System.Collections.Generic;
using System.Linq;
using Eagle._Components.Public;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;
using IptSimulator.CiscoTcl.Commands.Abstractions;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Model.EventArgs;
using IptSimulator.CiscoTcl.Utils;

namespace IptSimulator.CiscoTcl.Commands.Fsm
{
    public sealed class Fsm : CiscoTclCommand
    {
        private const string DefineCommand = "define";
        private const string SetStateCommand = "setstate";
        private const string RaiseEventCommand = "raise";

        private readonly HashSet<FsmTransition> _transitions = new HashSet<FsmTransition>();
        private string _overriddenNextState = string.Empty;
        private string _currentState;

        public Fsm() : base(
            new CommandData("fsm", null, null, null, typeof(Fsm).FullName, CommandFlags.None, null, 0))
        {
        }

        public Fsm(ICommandData commandData) : base(commandData)
        {
        }

        public string CurrentState
        {
            get { return _currentState; }
            private set
            {
                _currentState = value; 
                RaiseStateChangedEvent();
            }
        }

        protected override ReturnCode ExecuteInternal(Interpreter interpreter, IClientData clientData, ArgumentList arguments,
            ref Result result)
        {
            try
            {
                if (ValidateArguments(arguments, out result) != ReturnCode.Ok) return ReturnCode.Error;

                if (arguments[1] == DefineCommand)
                {
                    return ExecuteDefine(interpreter, arguments, ref result);
                }
                if (arguments[1] == SetStateCommand)
                {
                    return ExecuteSetState(arguments, ref result);
                }
                if (arguments[1] == RaiseEventCommand)
                {
                    return ExecuteRaiseEvent(interpreter, arguments, ref result);
                }

                var invalidArgs = $"Invalid argument {arguments[1]} in FSM command.";

                ErrorLogger.Error(invalidArgs);
                result = invalidArgs;
                return ReturnCode.Error;
            }
            catch (Exception e)
            {
                ErrorLogger.Error(e,"Unexpected error occured while executing Fsm command. " +
                                   $"Arguments: {(arguments == null ? "NULL" : string.Join(",",arguments))}");
                return ReturnCode.Error;
            }
        }

        private ReturnCode ExecuteRaiseEvent(Interpreter interpreter, ArgumentList arguments, ref Result result)
        {
            var raisedEvent = arguments[2];

            InternalLogger.Info($"Executing {raisedEvent} event.");

            if (!CiscoTclEvents.All.Contains(raisedEvent.String))
            {
                var unknownEvent = $"Event {raisedEvent} is not valid Cisco TCL event.\nValid CiscoTcl events are: [{string.Join(", ", CiscoTclEvents.All)}]";

                ErrorLogger.Error(unknownEvent);
                result = unknownEvent;
                return ReturnCode.Error;
            }
            
            var transition = DetermineCurrentTransition(raisedEvent);
            if (transition == null)
            {
                var transitionNotDefined = $"Transition for event {raisedEvent} is not defined in FSM.";

                ErrorLogger.Warn(transitionNotDefined);
                result = transitionNotDefined;
                //transition is not defined, but it's not an error, just don't perform any transitions
                return ReturnCode.Ok;
            }

            InternalLogger.Info($"Found transition for event {raisedEvent} is {transition}.");
            if (!TclUtils.ProcedureExists(interpreter, transition.Procedure))
            {
                var procedureNotExists = $"Procedure {transition.Procedure} which is defined in transition {transition} does not exist.";

                ErrorLogger.Error(procedureNotExists);
                result = procedureNotExists;
                return ReturnCode.Error;
            }

            var code = interpreter.EvaluateScript($"{transition.Procedure}", ref result);
            if (code != ReturnCode.Ok)
            {
                var error = $"Error occured while executing procedure {transition.Procedure}. Error: {result.String}";

                ErrorLogger.Error(error);
                result = error;
                return ReturnCode.Error;
            }

            var nextStateToSet = DetermineNextState(transition);
            if (!ContainsState(nextStateToSet))
            {
                var notDefinedState = $"Target state {nextStateToSet} defined in transition is not defined in FSM transitions.";

                ErrorLogger.Error(notDefinedState);
                result = notDefinedState;
                return ReturnCode.Error;
            }

            CurrentState = nextStateToSet;

            //unset overridden state no matter if it was or was not used
            _overriddenNextState = string.Empty;

            ResultLogger.Info($"Successfully raised {raisedEvent} and set FSM to state {nextStateToSet}");
            return ReturnCode.Ok;
        }

        private ReturnCode ExecuteDefine(Interpreter interpreter, ArgumentList arguments,
            ref Result result)
        {
            var fsmArray = arguments[2];
            var initialState = arguments[3];

            InternalLogger.Info($"Defining FSM with array {fsmArray} to initial state {initialState}");

            if (!TclUtils.ArrayExists(interpreter, fsmArray))
            {
                var fsmTransArrayError = $"Array of fsm states with name: {fsmArray} does not exist (is not defined).";

                ErrorLogger.Error(fsmTransArrayError);
                result = fsmTransArrayError;
                return ReturnCode.Error;
            }

            InternalLogger.Debug($"Retrieving all FSM transitions from {fsmArray} FSM array.");
            IReadOnlyList<FsmTransition> fsmTransitions;
            if (!FsmUtils.TryGetFsmTransitions(interpreter, fsmArray, out fsmTransitions))
            {
                var transitionsFailed = $"Failed to retrieve FSM transition from {fsmArray} array.";

                ErrorLogger.Error(transitionsFailed);
                result = transitionsFailed;
                return ReturnCode.Error;
            }

            InternalLogger.Info($"Assigning all {fsmTransitions.Count} into internal transitions set.");
            _transitions.Clear();
            foreach (var fsmTransition in fsmTransitions)
            {
                InternalLogger.Debug($"Adding FSM transition: {fsmTransition}");
                _transitions.Add(fsmTransition);
            }

            InternalLogger.Info($"Checking whether {initialState} state is defined in transition set.");
            if (!ContainsState(initialState))
            {
                var stateNotFound = $"State {initialState} was not found in defined transitions. Known states are:\n{string.Join(",", DumpStates())}";

                ErrorLogger.Error(stateNotFound);
                result = stateNotFound;
                return ReturnCode.Error;
            }

            CurrentState = initialState;
            ResultLogger.Info($"FSM successfully defined to initial state: {initialState}");
            return ReturnCode.Ok;
        }

        private ReturnCode ExecuteSetState(ArgumentList arguments, ref Result result)
        {
            var fsmState = arguments[2];

            InternalLogger.Info($"Executing FSM setstate with next state to be {fsmState}");

            if (!ContainsState(fsmState))
            {
                var stateNotFound = $"State {fsmState} was not found in defined transitions. Known states are:\n{string.Join(",", DumpStates())}";

                ErrorLogger.Error(stateNotFound);
                result = stateNotFound;
                return ReturnCode.Error;
            }


            _overriddenNextState = fsmState;
            ResultLogger.Info($"Next FSM state successfully set to be {fsmState}.");
            return ReturnCode.Ok;
        }

        protected override void PostExecute(Interpreter interpreter, IClientData clientData, ArgumentList arguments)
        {
            if (arguments[1] == DefineCommand)
            {
                //FSM is defined, raise event
                RaiseFsmGeneratedEvent();
            }
            if (arguments[1] == RaiseEventCommand)
            {
                //event was raised and current state *might* have changed
                //TODO: maybe add proper state change detection
                RaiseStateChangedEvent();
            }
        }

        #region Helper methods

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

        private string DetermineNextState(FsmTransition transition)
        {
            //next state MUST be determined after executing procedure, because in procedure there can be setstate

            string nextStateToSet;
            if (!string.IsNullOrEmpty(_overriddenNextState))
            {
                //if overridden state variable is set and we successfully get it's value, set it as next state
                nextStateToSet = _overriddenNextState;
            }
            else
            {
                //if not, just determine state from transition
                nextStateToSet = transition.DetermineActualTargetState();
            }
            return nextStateToSet;
        }

        private bool ContainsState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(state));

            return _transitions.Any(t => t.SourceState == state);
        }

        private FsmTransition DetermineCurrentTransition(string @event)
        {
            if (string.IsNullOrWhiteSpace(@event))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(@event));

            return _transitions.FirstOrDefault(t => t.SourceState == CurrentState && t.Event == @event);
        }

        private IEnumerable<string> DumpStates()
        {
            return _transitions
                .Select(t => t.SourceState)
                .Union(_transitions.Select(t => t.TargetState))
                .Distinct();
        }

        private void RaiseStateChangedEvent() => StateChanged?.Invoke(this, new FsmEventArgs(CurrentState, _transitions));

        private void RaiseFsmGeneratedEvent() => FsmGenerated?.Invoke(this, new FsmEventArgs(CurrentState, _transitions));

        #endregion

        public event EventHandler<FsmEventArgs> StateChanged;

        public event EventHandler<FsmEventArgs> FsmGenerated;
    }
}
