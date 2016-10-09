using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;

namespace IptSimulator.Core
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

        public static bool SetVariable(Interpreter interpreter, ref Result result, string variableName, string value = "")
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(variableName));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var code = interpreter.EvaluateScript($"set {variableName} {value}", ref result);

            return code == ReturnCode.Ok;
        }
    }
}
