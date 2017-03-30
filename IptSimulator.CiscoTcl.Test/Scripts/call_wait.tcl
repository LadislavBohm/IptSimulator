# provide call waiting

proc act_Setup { } {
  puts "\n---- in act_Setup \n"
  set ani [infotag get leg_ani]
  puts "\n---- ANI: $ani\n"
  leg proceeding leg_incoming
  leg progress leg_incoming
  leg alert leg_incoming
  leg connect leg_incoming
  set dnis [infotag get leg_dnis]
  puts "\n---- DNIS: $dnis\n"
  set callinfo(interceptEvents) "ev_alert"
  leg setup $dnis callinfo leg_incoming
}

proc act_SetupDone {} {
  puts "\n---- in act_SetupDone \n"
  set status [ infotag get evt_status ]
  puts "\n---- status: $status\n"
  switch $status {
    ls_000 {
      timer stop named_timer timer_wait
    }
    ls_007 {
      fsm setstate CALL_WAITING
      act_WaitPrompt
      act_StartTimer
    }
    default {
      leg disconnect leg_incoming -c 91
    }
  }
}

proc act_Cleanup { } {
  puts "\n---- in act_Cleanup \n"
  call close
}

proc act_WaitPrompt { } {
  puts "\n---- in act_WaitPrompt\n"
  media play leg_incoming _call_wait.au
}

proc act_StartTimer { } {
  global call_wait

  puts "\n---- in act_StartTimer\n"
  timer start named_timer $call_wait timer_wait
}

proc act_TimerExpired { } {
  puts "\n---- in act_TimerExpired\n"
  set tname [infotag get evt_timer_name]
  switch $tname {
    timer_wait {
      fsm setstate CALL_WAITING
      act_StartTimer
      act_TrySetupAgain
    }
    default {}
  }
}

proc act_TrySetupAgain { } {
  puts "\n---- in act_TrySetupAgain\n"
  set dnis [infotag get leg_dnis]
  puts "\n---- DNIS: $dnis\n"
  set callinfo(interceptEvents) "ev_alert"
  leg setup $dnis callinfo leg_incoming
}

proc act_WaitSetupDone {} {
  puts "\n---- in act_WaitSetupDone \n"
  set status [ infotag get evt_status ]
  puts "\n---- status: $status\n"
  switch $status {
    ls_000 {
      timer stop named_timer timer_wait
    }
    ls_007 {
      fsm setstate CALL_WAITING
      act_StartTimer
    }
    default {
      leg disconnect leg_incoming -c 91
    }
  }
}

proc act_Alert {} {
  puts "\n---- in act_Alert \n"
  timer stop named_timer timer_wait
  media stop leg_incoming
#  playtone leg_incoming tn_offhookalert
  set event_handle [infotag get evt_last_event_handle]
  leg setup_continue $event_handle -c callinfo
}

set call_wait 5

#----------------------------------
#   State Machine
#----------------------------------
set fsm(any_state,ev_disconnected)  "act_Cleanup,same_state"

set fsm(CALL_INIT,ev_setup_indication) "act_Setup,same_state"
set fsm(CALL_INIT,ev_alert) "act_Alert,same_state"
set fsm(CALL_INIT,ev_setup_done) "act_SetupDone,IN_CALL"
set fsm(CALL_WAITING,ev_named_timer) "act_TimerExpired,IN_CALL"
set fsm(CALL_WAITING,ev_media_done) "act_WaitPrompt,same_state"
set fsm(CALL_WAITING,ev_setup_done) "act_WaitSetupDone,IN_CALL"
set fsm(CALL_WAITING,ev_alert) "act_Alert,same_state"

fsm define fsm CALL_INIT

