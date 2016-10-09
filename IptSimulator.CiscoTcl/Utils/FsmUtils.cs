using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;
using NLog;

namespace IptSimulator.CiscoTcl.Utils
{
    public class FsmUtils
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static bool ContainsState(Interpreter interpreter, ref Result result, string fsmArray, string state)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (result == null) throw new ArgumentNullException(nameof(result));
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
    }
}
