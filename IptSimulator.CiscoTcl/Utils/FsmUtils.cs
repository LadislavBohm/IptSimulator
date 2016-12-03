using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Model;
using NLog;

namespace IptSimulator.CiscoTcl.Utils
{
    public class FsmUtils
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


        public static bool ContainsState(Eagle._Components.Public.Interpreter interpreter, ref Result result, string fsmArray, string state)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(fsmArray))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fsmArray));
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(state));

            Logger.Debug($"Checking whether FSM state {state} is present in FSM array {fsmArray}.");

            var code = interpreter.EvaluateScript($"array names {fsmArray}", ref result);

            if (code != ReturnCode.Ok)
            {
                Logger.Error($"Error while checking if FSM array {fsmArray} contains state {state}. Error: {result.String}");
                return false;
            }

            var states = result.String
                .Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(transition => transition.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            
            
            Logger.Debug($"Found FSM states: [{string.Join(", ", states)}]");

            var isPresent = states.Contains(state);
            Logger.Debug($"State {state} is {(isPresent ? string.Empty : "not")} persent in FSM array.");
            return isPresent;
        }

        public static bool TryGetFsmTransitions(Eagle._Components.Public.Interpreter interpreter, string fsmArray, out IReadOnlyList<FsmTransition> transitions)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(fsmArray))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fsmArray));

            transitions = new List<FsmTransition>();
            Result result = null;

            var code = interpreter.EvaluateScript($"array get {fsmArray}", ref result);

            if (code != ReturnCode.Ok)
            {
                Logger.Error($"Error while getting values from FSM array {fsmArray}. Error: {result.String}");
                return false;
            }

            //key value key value key value
            var arrayValues = result.String
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var output = new List<FsmTransition>();
            for (int i = 0; i < arrayValues.Length; i++)
            {
                if (i%2 != 0)
                {
                    var sourceStateWithEvent = arrayValues[i - 1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var procWithTargetState = arrayValues[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if(sourceStateWithEvent.Length != 2 || procWithTargetState.Length != 2) continue;

                    output.Add(new FsmTransition(
                        sourceStateWithEvent[0],
                        sourceStateWithEvent[1],
                        procWithTargetState[1],
                        procWithTargetState[0]));
                }
            }

            transitions = output;

            Logger.Info($"Found {output.Count} valid FSM transitions in array {fsmArray}.");
            Logger.Debug($"Found transitions are: \n{string.Join(Environment.NewLine,transitions.Select(t => t.ToString()))}");

            return true;
        }


    }
}
