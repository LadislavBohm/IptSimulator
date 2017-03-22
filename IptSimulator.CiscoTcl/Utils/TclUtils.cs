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
        public static bool ArrayExists(Interpreter interpreter, string arrayName)
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

        public static bool ProcedureExists(Interpreter interpreter, string procedureName)
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

        public static bool GetArrayValue(Interpreter interpreter, string arrayName, string key, ref Result result)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (arrayName == null) throw new ArgumentNullException(nameof(arrayName));
            if (key == null) throw new ArgumentNullException(nameof(key));

            const string tempVariableName = "A51947B5F3C9403F81DD0DC286E781E3";
            ReturnCode code = interpreter.EvaluateScript($"set {tempVariableName} ${arrayName}({key})", ref result);

            Result cleanupResult = null;
            UnsetVariable(interpreter, ref cleanupResult, tempVariableName);

            return code == ReturnCode.Ok;
        }

        public static bool ArrayKeyExists(Interpreter interpreter, string arrayName, string key)
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

        public static bool ReadArray(Interpreter interpreter, ref IDictionary<string, string> result, string arrayName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (arrayName == null) throw new ArgumentNullException(nameof(arrayName));

            Result tclResult = "";
            var code = interpreter.EvaluateScript($"array names {arrayName}", ref tclResult);

            if (code != ReturnCode.Ok)
            {
                return false;
            }

            var arrayNames = tclResult.ToString().Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var name in arrayNames)
            {
                if (GetArrayValue(interpreter, arrayName, name, ref tclResult))
                {
                    result.Add(name, tclResult);
                }
            }

            return true;
        }

        public static bool SetVariable(Interpreter interpreter, ref Result result, string variableName, string value, bool global)
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

        public static bool UnsetVariable(Interpreter interpreter, ref Result result, string variableName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));

            var code = interpreter.EvaluateScript($"unset {variableName}", ref result);

            return code == ReturnCode.Ok;
        }

        public static bool GetVariableValue(Interpreter interpreter, string variableName, bool global, ref VariableWithValue result)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));

            var scopedName = $"{(global ? "::" : string.Empty)}{variableName}";
            ReturnCode code;
            Result tclResult = null;
            if (ArrayExists(interpreter, scopedName))
            {
                IDictionary<string, string> arrayResult = new Dictionary<string, string>();
                code = ReadArray(interpreter, ref arrayResult, scopedName) ? ReturnCode.Ok : ReturnCode.Error;
                var singleResult = string.Join(", ", arrayResult.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                result = new VariableWithValue(variableName, singleResult, arrayResult.Select(kvp => new VariableWithValue(kvp.Key, kvp.Value)));
            }
            else
            {
                code = interpreter.EvaluateScript($"set {scopedName}", ref tclResult);
                result = new VariableWithValue(variableName, tclResult);
            }

            return code == ReturnCode.Ok;
        }

        public static IReadOnlyCollection<VariableWithValue> GetVariableValues(Interpreter interpreter, bool excludeReserved)
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
                VariableWithValue variableWithValue = null;
                if (GetVariableValue(interpreter, variable, true, ref variableWithValue) &&
                    !string.IsNullOrWhiteSpace(variableWithValue.Value))
                {
                    variablesWithValues.Add(variableWithValue);
                }
            }

            return variablesWithValues;
        }

        public static bool VariableExists(Interpreter interpreter, ref Result result, string variableName)
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
