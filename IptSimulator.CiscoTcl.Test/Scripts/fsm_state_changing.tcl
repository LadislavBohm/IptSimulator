proc some_procedure { } {

}

set fsm(FIRST_STATE,ev_any_event) "some_procedure,same_state"
set fsm(SECOND_STATE,ev_any_event) "some_procedure,same_state"
set fsm(THIRD_STATE,ev_any_event) "some_procedure,same_state"
set fsm(FOURTH_STATE,ev_any_event) "some_procedure,same_state"
set fsm(FIFTH_STATE,ev_any_event) "some_procedure,same_state"

fsm define fsm SECOND_STATE

fsm setstate THIRD_STATE
fsm setstate FIFTH_STATE
fsm setstate SECOND_STATE
fsm setstate FIRST_STATE
fsm setstate FOURTH_STATE

fsm raise ev_any_event