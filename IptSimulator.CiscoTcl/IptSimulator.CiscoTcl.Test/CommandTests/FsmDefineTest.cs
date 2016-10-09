using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Commands;
using Xunit;

namespace IptSimulator.CiscoTcl.Test.CommandTests
{
    public class FsmDefineTest : InterpreterTestBase
    {
        [Theory,
            InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                       "fsm define fsm CALL_INIT", "CALL_INIT"),
            InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                       "set fsm(any_state,ev_disconnected)  \"act_Cleanup,same_state\"\n" +
                       "fsm define fsm any_state", "any_state")]
        public void DefineFsm_ValidArguments_ReturnOk(string script, string expectedResult)
        {
            Result result = null;
            int errorLine = 0;
            long token = 0;

            using (var interpreter = Interpreter.Create(ref result))
            {
                var code = interpreter.AddCommand(new FsmDefine(), null, ref token, ref result);

                Assert.Equal(ReturnCode.Ok,code);

                code = interpreter.EvaluateScript(script, ref result, ref errorLine);

                Assert.Equal(ReturnCode.Ok, code);
                Assert.NotNull(result);
                Assert.Equal(ResultFlags.String,result.Flags);
                Assert.Contains(expectedResult,result.String);
            }
        }

        [Theory, 
            InlineData("fsm define fsm"), 
            InlineData("fsm define"), 
            InlineData("fsm define any_state")]
        public void DefineFsm_MissingArguments_ReturnError(string script)
        {
            Result result = null;
            int errorLine = 0;
            long token = 0;

            using (var interpreter = Interpreter.Create(ref result))
            {
                var code = interpreter.AddCommand(new FsmDefine(), null, ref token, ref result);

                Assert.Equal(ReturnCode.Ok, code);
                code = interpreter.EvaluateScript(script, ref result, ref errorLine);

                Assert.Equal(ReturnCode.Error, code);
                Assert.NotNull(result);
            }
        }

        [Theory, 
            InlineData("fsm define fsm any_state same_state"),
            InlineData("fsm define a a a a a a"),
            InlineData("fsm define test test test fsm")]
        public void DefineFsm_TooManyArguments_ReturnError(string script)
        {
            Result result = null;
            int errorLine = 0;
            long token = 0;

            using (var interpreter = Interpreter.Create(ref result))
            {
                var code = interpreter.AddCommand(new FsmDefine(), null, ref token, ref result);

                Assert.Equal(ReturnCode.Ok, code);
                code = interpreter.EvaluateScript(script, ref result, ref errorLine);

                Assert.Equal(ReturnCode.Error, code);
                Assert.NotNull(result);
            }
        }

        public void DefineFsm_NonExistingInitState_ReturnError(string script)
        {
            
        }
    }
}
