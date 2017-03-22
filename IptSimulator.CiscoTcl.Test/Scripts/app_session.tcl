#------------------------------------------------------------------
# August 1999, Saravanan Shanmugham
#
# Copyright (c) 1998, 1999 by cisco Systems, Inc.
# All rights reserved.
#------------------------------------------------------------------
#
# This tcl script mimics the default SESSION app
#
#
# If DID is configured, just place the call to the dnis
# Otherwise, output dial-tone and collect digits from the
# caller against the dial-plan.
#
# Then place the call. If successful, connect it up, otherwise
# the caller should hear a busy or congested signal.
# The main routine just establishes the state machine and then exits.
# From then on the system drives the state machine depending on the
# events it receives and calls the appropriate tcl procedure
#---------------------------------
# Example Script
#---------------------------------
proc init { } {
	global param
	set param(interruptPrompt) true
	set param(abortKey) *
	set param(terminationKey) #
}
proc act_Setup { } {
	global dest
	global beep
	set beep 0
	leg setupack leg_incoming
	if { [infotag get leg_isdid] } {
		set dest [infotag get leg_dnis]
		leg proceeding leg_incoming
		leg setup $dest callInfo leg_incoming
		fsm setstate PLACECALL
	} else {
		playtone leg_incoming tn_dial
		set param(dialPlan) true
		leg collectdigits leg_incoming param
	}
}

proc act_GotDest { } {
	global dest
	set status [infotag get evt_status]
	if { $status == "cd_004" } {
		set dest [infotag get evt_dcdigits]
		leg proceeding leg_incoming
		leg setup $dest callInfo leg_incoming
	} else {
		puts "\nCall [infotag get con_all] got event $status while placing an outgoing
		call"
		call close
	}
}

proc act_CallSetupDone { } {
	global beep
	set status [infotag get evt_status]
	if { $status == "CS_000"} {
		set creditTimeLeft [infotag get leg_settlement_time leg_outgoing]
		if { ($creditTimeLeft == "unlimited") ||
			($creditTimeLeft == "uninitialized") } {
			puts "\n Unlimited Time"
		} else {
			# start the timer for ...
			if { $creditTimeLeft < 10 } {
				set beep 1
				set delay $creditTimeLeft
			} else {
				set delay [expr $creditTimeLeft - 10]
			}
			timer start leg_timer $delay leg_incoming
		}
	} else {
		puts "Call [infotag get con_all] got event $status collecting destination"
		call close
	}
}

proc act_Timer { } {
	global beep
	global incoming
	global outgoing
	set incoming [infotag get leg_incoming]
	set outgoing [infotag get leg_outgoing]
	if { $beep == 0 } {
		#insert a beep ...to the caller
		connection destroy con_all
		set beep 1
	} else {
		media play leg_incoming flash:out_of_time.au
		fsm setstate CALLDISCONNECTED
	}
}

proc act_Destroy { } {
	media play leg_incoming flash:beep.au
}

proc act_Beeped { } {
	global incoming
	global outgoing
	connection create $incoming $outgoing
}

proc act_ConnectedAgain { } {
	timer start leg_timer 10 leg_incoming
}
proc act_Ignore { } {
	# Dummy
	puts "Event Capture"
}

proc act_Cleanup { } {
	call close
}

init

#----------------------------------
# State Machine
#----------------------------------
set TopFSM(any_state,ev_disconnected) "act_Cleanup,same_state"
set TopFSM(CALL_INIT,ev_setup_indication) "act_Setup,GETDEST"
set TopFSM(GETDEST,ev_digitcollect_done) "act_GotDest,PLACECALL"
set TopFSM(PLACECALL,ev_setup_done) "act_CallSetupDone,CALLACTIVE"
set TopFSM(CALLACTIVE,ev_leg_timer) "act_Timer,INSERTBEEP"
set TopFSM(INSERTBEEP,ev_destroy_done) "act_Destroy,same_state"
set TopFSM(INSERTBEEP,ev_media_done) "act_Beeped,same_state"
set TopFSM(INSERTBEEP,ev_create_done) "act_ConnectedAgain,CALLACTIVE"
set TopFSM(CALLACTIVE,ev_disconnected) "act_Cleanup,CALLDISCONNECTED"
set TopFSM(CALLDISCONNECTED,ev_disconnected) "act_Cleanup,same_state"
set TopFSM(CALLDISCONNECTED,ev_media_done) "act_Cleanup,same_state"
set TopFSM(CALLDISCONNECTED,ev_media_done) "act_Cleanup,same_state"
set TopFSM(CALLDISCONNECTED,ev_disconnect_done) "act_Cleanup,same_state"
set TopFSM(CALLDISCONNECTED,ev_leg_timer) "act_Cleanup,same_state"

fsm define TopFSM CALL_INIT