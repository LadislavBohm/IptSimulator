using Eagle._Components.Public;
using IptSimulator.CiscoTcl.Commands;
using Xunit;

namespace IptSimulator.CiscoTcl.Test.CommandTests
{
    public class FsmTest : InterpreterTestBase
    {
        #region Inline data tests

        #region DefineFsm tests

        [Theory,
         InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "fsm define fsm CALL_INIT", "CALL_INIT"),
         InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set fsm(any_state,ev_disconnected)  \"act_Cleanup,same_state\"\n" +
                    "fsm define fsm any_state", "any_state")]
        public void DefineFsm_ValidArguments_ReturnOk(string script, string expectedResult)
        {
            EvaluateAndExpectSuccess(script, expectedResult, new Fsm());
        }

        [Theory,
         InlineData("fsm define FSM"), 
         InlineData("fsm define"), 
         InlineData("fsm define any_state")]
        public void DefineFsm_MissingArguments_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory, 
         InlineData("fsm define fsm any_state same_state"),
         InlineData("fsm define a a a a a a"),
         InlineData("fsm define test test test fsm")]
        public void DefineFsm_TooManyArguments_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory,
         InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set fsm(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define fsm INIT_STATE\n")]
        public void DefineFsm_NontExistingFsmArray_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory,
         InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set fsm(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define fsm INIT_STATE\n")]
        public void DefineFsm_NonExistingInitState_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }
            #endregion

        #region SetState tests

        [Theory,
         InlineData("set test(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set test(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define test CALL_WAITING\n" +
                    "fsm setstate"),
         InlineData("set test(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set test(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define test CALL_WAITING\n" +
                    "fsm setstate xxx yyyy")]
        public void SetStateFsm_InvalidArguments_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory,
         InlineData("set test(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set test(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define test CALL_INIT\n" +
                    "fsm setstate CALL_WAITING", "CALL_WAITING")]
        public void SetStateFsm_ValidArguments_ReturnOk(string script, string expectedResult)
        {
            EvaluateAndExpectSuccess(script,expectedResult, new Fsm());
        }

        [Theory,
         InlineData("set test(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set test(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define test CALL_WAITING\n" +
                    "fsm setstate CALL_WAITING", "CALL_WAITING")]
        public void SetStateFsm_SetSameStateAsInit_ReturnOk(string script, string expectedResult)
        {
            EvaluateAndExpectSuccess(script, expectedResult, new Fsm());
        }

        [Theory,
         InlineData("set test(CALL_INIT,ev_setup_indication) \"act_Setup, same_state\"\n" +
                    "set test(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define test CALL_WAITING\n" +
                    "fsm setstate xxx")]
        public void SetStateFsm_NonExistingState_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }
            #endregion

        #region Raise Event tests

        [Theory,
         InlineData("fsm raise test_event"),
         InlineData("fsm raise neco"),
         InlineData("fsm raise test_event test_event")]
        public void RaiseEvent_InvalidEvent_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory,
         InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup,same_state\"\n" +
                    "fsm define fsm CALL_INIT\n" +
                    "fsm raise ev_address_resolved")]
        public void RaiseEvent_NonExistingTransition_ReturnOk(string script)
        {
            EvaluateAndExpectSuccess(script, new Fsm());
        }

        [Theory,
         InlineData("set fsm(CALL_INIT,ev_setup_indication) \"act_Setup,same_state\"\n" +
                    "fsm define fsm CALL_INIT\n" +
                    "fsm raise ev_setup_indication")]
        public void RaiseEvent_NonExistingProcedureInTransition_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory,
         InlineData("proc act_Setup { } { } \n" +
                    "set fsm(CALL_INIT,ev_setup_indication) \"act_Setup,CALL_WAITING\"\n" +
                    "set fsm(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                    "fsm define fsm CALL_INIT\n" +
                    "fsm raise ev_setup_indication")]
        public void RaiseEvent_ValidTransition_ReturnOk(string script)
        {
            EvaluateAndExpectSuccess(script, new Fsm());
        }

        [Theory,
            InlineData("proc act_Setup { } { } \n" +
                "set fsm(CALL_INIT,ev_setup_indication) \"act_Setup,same_state\"\n" +
                "set fsm(CALL_WAITING,ev_named_timer) \"act_TimerExpired,IN_CALL\"\n" +
                "fsm define fsm CALL_INIT\n" +
                "fsm raise ev_setup_indication")]
        public void RaiseEvent_ValidTransitionWithSpecialState_ReturnOk(string script)
        {
            EvaluateAndExpectSuccess(script, new Fsm());
        }

        [Theory,
            InlineData("proc act_Setup { } { } \n" +
                "set fsm(CALL_INIT,ev_setup_indication) \"act_Setup,NON_EXISTING_STATE\"\n" +
                "fsm define fsm CALL_INIT\n" +
                "fsm raise ev_setup_indication")]
        public void RaiseEvent_NotDefinedStateInTransition_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }

        [Theory,
            InlineData("proc act_Setup { } { } \n" +
                       "set fsm(CALL_INIT,ev_setup_indication) \"act_Setup,any_state\"\n" +
                       "fsm define fsm CALL_INIT\n" +
                       "fsm raise ev_setup_indication")]
        public void RaiseEvent_ValidTransitionWithInvalidSpecialState_ReturnError(string script)
        {
            EvaluateAndExpectError(script, new Fsm());
        }
        
        #endregion

        #endregion

        #region Integration tests

        [Fact]
        public void ChangingFsmState_FromScript_FourthState()
        {
            EvaluateAndExpectSuccess(ScriptProvider.FsmStateChanging,"FOURTH_STATE", new Fsm());
        }

        #endregion

    }
}
