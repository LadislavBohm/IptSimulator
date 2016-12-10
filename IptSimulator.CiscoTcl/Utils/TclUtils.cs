using System;
using System.Collections.Generic;
using System.Linq;
using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.Core.Tcl;

namespace IptSimulator.CiscoTcl.Utils
{
    public class TclUtils
    {
        public static bool ArrayExists(Eagle._Components.Public.Interpreter interpreter, string arrayName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrEmpty(arrayName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(arrayName));

            Result result = null;
            ReturnCode code = interpreter.EvaluateScript($"array exists {arrayName}", ref result);

            if (code != ReturnCode.Ok)
            {
                return false;
            }

            return (bool)result.Value;
        }

        public static bool ProcedureExists(Eagle._Components.Public.Interpreter interpreter, string procedureName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrEmpty(procedureName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(procedureName));

            Result result = null;
            ReturnCode code = interpreter.EvaluateScript($"info procs {procedureName}", ref result);

            if (code != ReturnCode.Ok)
            {
                return false;
            }
            return !string.IsNullOrEmpty(result.String);
        }

        public static bool ArrayKeyExists(Eagle._Components.Public.Interpreter interpreter, string arrayName, string key)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            if (string.IsNullOrWhiteSpace(arrayName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(arrayName));

            Result result = null;
            ReturnCode code = interpreter.EvaluateScript($"info exists {arrayName}({key})", ref result);

            if (code != ReturnCode.Ok)
            {
                return false;
            }

            return result.String == "1";
        }

        public static bool SetVariable(Eagle._Components.Public.Interpreter interpreter, ref Result result, string variableName, string value, bool global)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

            string defineAsGlobal = global ? $"global {variableName}" : string.Empty;
            var code = interpreter.EvaluateScript($"{defineAsGlobal}\nset {variableName} {value}", ref result);

            return code == ReturnCode.Ok;
        }

        public static bool UnsetVariable(Eagle._Components.Public.Interpreter interpreter, ref Result result, string variableName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));

            var code = interpreter.EvaluateScript($"unset {variableName}", ref result);

            return code == ReturnCode.Ok;
        }

        public static bool GetVariableValue(Eagle._Components.Public.Interpreter interpreter, ref Result result, string variableName, bool global)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));

            var code = interpreter.EvaluateScript($"set {(global ? "::" : string.Empty)}{variableName}", ref result);

            return code == ReturnCode.Ok;
        }

        public static IReadOnlyCollection<VariableWithValue> GetVariableValues(Eagle._Components.Public.Interpreter interpreter, bool excludeReserved)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));

            Result result = null;
            var code = interpreter.EvaluateScript("info vars", ref result);

            if (code != ReturnCode.Ok)
            {
                return Enumerable.Empty<VariableWithValue>().ToList();
            }

            var variables = result.String.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var variablesWithValues = new List<VariableWithValue>();

            foreach (var variable in variables)
            {
                if (excludeReserved && TclReservedVariables.All.Contains(variable))
                {
                    //exclude reserved variable
                    continue;
                }
                if (GetVariableValue(interpreter, ref result, variable, true) && !string.IsNullOrWhiteSpace(result))
                {
                    variablesWithValues.Add(new VariableWithValue(variable, result.String));
                }
            }

            return variablesWithValues;
        }

        public static bool VariableExists(Eagle._Components.Public.Interpreter interpreter, ref Result result, string variableName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));

            var code = interpreter.EvaluateScript($"info exists {variableName}", ref result);


            if (code != ReturnCode.Ok)
            {
                return false;
            }

            return result.String == "1";
        }
    }
}
