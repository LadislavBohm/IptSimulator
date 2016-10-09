using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Components.Public;

namespace IptSimulator.Core
{
    public static class TclExtensions
    {
        public static bool ArrayExists(this Interpreter interpreter, string arrayName)
        {
            if (string.IsNullOrEmpty(arrayName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(arrayName));

            Result result = null;
            ReturnCode code = interpreter.EvaluateScript($"array exists {arrayName}", ref result);

            if (code != ReturnCode.Ok)
            {
                return false;
            }

            return string.Equals("1", result.String);
        }

        public static bool ProcedureExists(this Interpreter interpreter, string procedureName)
        {
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
    }
}
