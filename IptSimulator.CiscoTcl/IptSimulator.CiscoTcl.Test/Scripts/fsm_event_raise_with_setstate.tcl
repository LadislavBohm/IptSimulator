set fsm(FIRST_STATE,ev_any_event) "some_procedure,same_state"
set fsm(SECOND_STATE,ev_any_event) "some_procedure,FIFTH_STATE"
set fsm(THIRD_STATE,ev_any_event) "some_procedure,same_state"
set fsm(FOURTH_STATE,ev_any_event) "some_procedure,same_state"
set fsm(FIFTH_STATE,ev_any_event) "some_procedure,same_state"

fsm define fsm SECOND_STATE

proc some_procedure { } {
	variable fsm
	fsm setstate THIRD_STATE	

}



fsm raise ev_any_event