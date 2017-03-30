using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle._Commands;
using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Commands;
using NLog;
using NLog.Config;
using Xunit;

namespace IptSimulator.CiscoTcl.Test
{
    public abstract class InterpreterTestBase
    {
        protected readonly ScriptDataProvider ScriptProvider = new ScriptDataProvider();

        protected InterpreterTestBase()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
        }

        protected void EvaluateAndExpectError(string script, params Default[] commands)
        {
            Result result = null;
            int errorLine = 0;
            long token = 0;

            using (var interpreter = Interpreter.Create(ref result))
            {
                ReturnCode code;
                foreach (var command in commands)
                {
                    code = interpreter.AddCommand(command, null, ref token, ref result);
                    Assert.Equal(ReturnCode.Ok, code);
                }

                code = interpreter.EvaluateScript(script, ref result, ref errorLine);

                Assert.Equal(ReturnCode.Error, code);
                Assert.NotNull(result);
                Assert.Null(result.Exception);
            }
        }

        protected void EvaluateAndExpectSuccess(string script, params Default[] commands)
        {
            Result result = null;
            int errorLine = 0;
            long token = 0;

            using (var interpreter = Interpreter.Create(ref result))
            {
                ReturnCode code;
                foreach (var command in commands)
                {
                    code = interpreter.AddCommand(command, null, ref token, ref result);
                    Assert.Equal(ReturnCode.Ok, code);
                }

                code = interpreter.EvaluateScript(script, ref result, ref errorLine);

                Assert.Equal(ReturnCode.Ok, code);
            }
        }

        protected void EvaluateAndExpectSuccess(string script, Func<bool> finalAssert, params Default[] commands)
        {
            Result result = null;
            int errorLine = 0;
            long token = 0;

            using (var interpreter = Interpreter.Create(ref result))
            {
                ReturnCode code;
                foreach (var command in commands)
                {
                    code = interpreter.AddCommand(command, null, ref token, ref result);
                    Assert.Equal(ReturnCode.Ok, code);
                }

                code = interpreter.EvaluateScript(script, ref result, ref errorLine);

                Assert.Equal(ReturnCode.Ok, code);
                Assert.True(finalAssert());
            }
        }
    }
}
