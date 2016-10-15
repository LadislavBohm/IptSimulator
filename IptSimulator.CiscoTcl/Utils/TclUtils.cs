using System;
using System.Collections.Generic;
using System.Linq;
using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Model;

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

        public static bool SetVariable(Interpreter interpreter, ref Result result, string variableName, string value)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

            var code = interpreter.EvaluateScript($"set {variableName} {value}", ref result);

            return code == ReturnCode.Ok;
        }

        public static bool GetVariableValue(Interpreter interpreter, ref Result result, string variableName)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));

            var code = interpreter.EvaluateScript($"set {variableName}", ref result);

            return code == ReturnCode.Ok;
        }

        public static IReadOnlyCollection<VariableWithValue> GetVariableValues(Interpreter interpreter)
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
                if (GetVariableValue(interpreter, ref result, variable) && !string.IsNullOrWhiteSpace(result))
                {
                    variablesWithValues.Add(new VariableWithValue(variable,result.String));
                }
            }

            return variablesWithValues;
        }
    }
}
