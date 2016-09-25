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

namespace IptSimulator.CiscoTcl.Commands
{
    public class FsmDefine : CiscoTclCommand
    {
        public FsmDefine() : base(
            new CommandData("fsm define", null, null, null, typeof(FsmDefine).FullName, CommandFlags.None, null, 0))
        {
        }

        public FsmDefine(ICommandData commandData) : base(commandData)
        {
        }

        public override ReturnCode Execute(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            if ((arguments == null) || (arguments.Count != 4))
            {
                result = Utility.WrongNumberOfArguments(this, 2, arguments, "fsm_array initial_state");

                return ReturnCode.Error;
            }

            var fsmArray = arguments[2];
            var initialState = arguments[3];

            if (!ArrayExists(interpreter, fsmArray))
            {
                var arrayNotExists = $"Array of fsm states with name: {fsmArray} does not exist (is not defined).";

                BaseLogger.Error(arrayNotExists);
                result = arrayNotExists;

                return ReturnCode.Error;
            }


            result = $"Defining initial FSM state: {arguments.Last()}";
            return ReturnCode.Ok;
        }

        private bool ArrayExists(Interpreter interpreter, string arrayName)
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

            return string.Equals("1", result.String);
        }

        private bool ProcedureExists(Interpreter interpreter, string procedureName)
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
    }
}
