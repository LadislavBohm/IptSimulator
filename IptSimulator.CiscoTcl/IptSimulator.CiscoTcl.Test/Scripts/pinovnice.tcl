# *********************************
# Conference
# Version 2.0
# *********************************
# Copyright (c) 2013 ALEF NULA a.s.
# *********************************

# *********************************
# CallInitReqLanguage
# *********************************
proc CallInitReqLanguage { } {
        global _prompts
        global _collectDigitsParams
        global Params
        global _languageNotFilledCount
        global _pinNotFilledCount
        global _attendeeId
        global _pin
        global _phoneNumber
        global _conferenceNumber
        
        leg setupack leg_incoming
        leg proceeding leg_incoming
        leg connect leg_incoming

        GetParamBool Debug false
        GetParamString PhonePrefix "9567"
        
        GetParamString ServerUrl1 "http://10.144.252.64/TeleKonference/"
        GetParamString ServerUrl2 "http://10.145.252.64/TeleKonference/"
        
        GetParamString PageUrl "ConferenceNumber.aspx"
        GetParamString AttendeeConnectedPageUrl "Tcl/ConferenceAttendeeConnected.aspx"
        GetParamString AttendeeDisconnectedPageUrl "Tcl/ConferenceAttendeeDisconnected.aspx"
        GetParamString PhoneNumberBlacklistedUrl "Tcl/PhoneNumberBlacklisted.aspx"
        GetParamInt MaxLanguageNotFilledCount 2
        GetParamInt MaxPinNotFilledCount 3
        GetParamInt DigitCollectTimeout 15
        GetParamInt HttpTimeout 2
        GetParamString HelpdeskNumber "1811"
        PrintDebug "Params(Debug): $Params(Debug)"
        PrintDebug "Params(PhonePrefix): $Params(PhonePrefix)"
        PrintDebug "Params(ServerUrl1): $Params(ServerUrl1)"
        PrintDebug "Params(ServerUrl2): $Params(ServerUrl2)"
        PrintDebug "Params(PageUrl): $Params(PageUrl)"
        PrintDebug "Params(AttendeeConnectedPageUrl): $Params(AttendeeConnectedPageUrl)"
        PrintDebug "Params(AttendeeDisconnectedPageUrl): $Params(AttendeeDisconnectedPageUrl)"
        PrintDebug "Params(PhoneNumberBlacklistedUrl): $Params(PhoneNumberBlacklistedUrl)"
        PrintDebug "Params(MaxLanguageNotFilledCount): $Params(MaxLanguageNotFilledCount)"
        PrintDebug "Params(MaxPinNotFilledCount): $Params(MaxPinNotFilledCount)"
        PrintDebug "Params(DigitCollectTimeout): $Params(DigitCollectTimeout)"
        PrintDebug "Params(HttpTimeout): $Params(HttpTimeout)"
        PrintDebug "Params(HelpdeskNumber): $Params(HelpdeskNumber)"
        
        set _prompts(greeting_all) "flash://konference/telekon_1.wav"
        set _prompts(enterPin_cz) "flash://konference/telekon_2.wav"
        set _prompts(enterPin_en) "flash://konference/telekon_3.wav"
        set _prompts(confNotFound_cz) "flash://konference/telekon_4.wav"
        set _prompts(confNotFound_en) "flash://konference/telekon_5.wav"
        set _prompts(transfering_cz) "flash://konference/telekon_6.wav"
        set _prompts(transfering_en) "flash://konference/telekon_7.wav"
        set _prompts(transferError_all) "flash://konference/telekon_3.wav"
        set _prompts(confFull_cz) "flash://konference/telekon_8.wav"
        set _prompts(confFull_en) "flash://konference/telekon_9.wav"
        set _prompts(confNotOpenYet_cz) "flash://konference/telekon_13.wav"
        set _prompts(confNotOpenYet_en) "flash://konference/telekon_14.wav"
        set _prompts(conFirstAttendee_cz) "flash://konference/telekon_11.wav"
        set _prompts(conFirstAttendee_en) "flash://konference/telekon_12.wav"
        set _prompts(numberIsBlocked_cz) "flash://konference/telekon_15.wav"
        set _prompts(numberIsBlocked_en) "flash://konference/telekon_16.wav"
        
        
        set _collectDigitsParams(interDigitTimeout) $Params(DigitCollectTimeout)
        set _collectDigitsParams(consumeDigit) true
        set _collectDigitsParams(maxDigits) 8
        set _collectDigitsParams(initialDigitTimeout) $Params(DigitCollectTimeout)
        set _collectDigitsParams(interruptPrompt) true
        set _collectDigitsParams(terminationKey) "#"

        set _languageNotFilledCount 0
        set _pinNotFilledCount 0
        if {[info exists _attendeeId]} {unset _attendeeId}
        if {[info exists _pin]} {unset _pin}
        if {[info exists _conferenceNumber]} {unset _conferenceNumber}
        if {[info exists _phoneNumber]} {unset _phoneNumber}
        
        
        RequestLanguage
}

proc RequestLanguage { } {
        global _prompts
        global _collectDigitsParams
        media play leg_incoming $_prompts(greeting_all)
        leg collectdigits leg_incoming _collectDigitsParams
}

# *********************************
# close call
# *********************************
proc CloseCall { } {
        PrintDebug "CloseCall"
        call close
}

# *********************************
# SetLanguageCheckBlockedNumber
# *********************************
proc SetLanguageCheckBlockedNumber { } {
        global Params
        global _prompts
        global _collectDigitsParams
        global _language
        global _languageNotFilledCount
        
        set _language [infotag get evt_dcdigits]
        if { ![info exists _language] || $_language == "" } {
                set _languageNotFilledCount [expr {$_languageNotFilledCount + 1}]
                PrintDebug "Language code wasn't entered for the $_languageNotFilledCount. time."
                if { $_languageNotFilledCount >= $Params(MaxLanguageNotFilledCount) } {
                        set _language 1
                } else {
                        RequestLanguage
                        fsm setstate LanguageSelected
                        return
                }
        }
        CheckBlockedNumber 1
}

# *********************************
# CheckBlockedNumber
# *********************************
proc CheckBlockedNumber { isFirstStep } {
        global isHttpCallbackBlocking
        global isHttpCallbackBlockingSecondStep
        global isSuccessResponseBlocking
        global isSuccessResponseBlockingSecondStep
        global _phoneNumber2
        global Params
        set _phoneNumber2 [infotag get leg_ani]
        
        PrintDebug "CheckBlockedNumber isFirstStep='$isFirstStep'" 
        PrintDebug "Call incoming from: '$_phoneNumber2'"
        
        set isHttpCallbackBlocking 0
        set isHttpCallbackBlockingSecondStep 0  
        set isSuccessResponseBlocking "false"
        set isSuccessResponseBlockingSecondStep "false"
        set url $Params(ServerUrl1)
        append url $Params(PhoneNumberBlacklistedUrl)
        append url "?phoneNumber=" $_phoneNumber2
        if { $isFirstStep == 1 } {
                ::httpios::geturl $url -command HttpCallbackBlocking -timeout $Params(HttpTimeout)
        } else {
                ::httpios::geturl $url -command HttpCallbackBlockingSecondStep -timeout $Params(HttpTimeout)
        }
        
        set url $Params(ServerUrl2)
        append url $Params(PhoneNumberBlacklistedUrl)
        append url "?phoneNumber=" $_phoneNumber2
        if { $isFirstStep == 1 } {
                ::httpios::geturl $url -command HttpCallbackBlocking -timeout $Params(HttpTimeout)
        } else {
                ::httpios::geturl $url -command HttpCallbackBlockingSecondStep -timeout $Params(HttpTimeout)
        }
        
}

# *********************************
# HttpCallbackBlocking
# *********************************
proc HttpCallbackBlocking {token} {
        global isHttpCallbackBlocking
        global isSuccessResponseBlocking
        incr isHttpCallbackBlocking
        PrintDebug "HttpCallbackBlocking, CallbackCount='$isHttpCallbackBlocking', isSuccessResponseBlocking='$isSuccessResponseBlocking' "
        PrintDebug "Status: '[::httpios::status $token]', ResponseCode: '[::httpios::ncode $token]', Data: '[::httpios::data $token]'"          
        
        if { [::httpios::ncode $token] == "200" } {
                if { $isSuccessResponseBlocking == "false" } {  
                        set isSuccessResponseBlocking "true"
                        CheckBlockReqPin [::httpios::data $token]
                } else { 
                        PrintDebug "HttpCallbackBlocking: Ignore second callback"
                }
        } else {
                if { $isHttpCallbackBlocking == 2 && $isSuccessResponseBlocking == "false" } {
                        PlayError
                        fsm setstate ErrorNotifi
                } else {
                        PrintDebug "HttpCallbackBlocking: Ignore this callback"
                }
        }

        ::httpios::cleanup $token
}

# *********************************
# CheckBlockReqPin
# *********************************
proc CheckBlockReqPin { isBlacklisted } {
        
# if result 0 -> RequestPin
# else -> play error and set state NumberIsBlocked

        global _prompts
        global _language
        global _collectDigitsParams
        
        PrintDebug "CheckBlockReqPin '$isBlacklisted'"

        if { $isBlacklisted == "" } {
                PrintDebug "isBlacklisted param is empty. Error"
                fsm setstate ErrorNotifi
                return
        }
        
        if { $isBlacklisted == "NotBlacklisted"  } {
                RequestPin
        } else {
                if  { $_language == 2 } {
                        media play leg_incoming $_prompts(numberIsBlocked_en)
                        fsm setstate NumberIsBlocked
                } else {
                        media play leg_incoming $_prompts(numberIsBlocked_cz)
                        fsm setstate NumberIsBlocked
                }
        }
}

# *********************************
# RequestPin
# *********************************
proc RequestPin { } {
        global _prompts
        global _collectDigitsParams
        global _language
        PrintDebug "language: '$_language'"
        if  { $_language == 2} {
                media play leg_incoming $_prompts(enterPin_en)
        } else {
                media play leg_incoming $_prompts(enterPin_cz)
        }
        leg collectdigits leg_incoming _collectDigitsParams
}

# *********************************
# SetPinFindConfNumber
# *********************************
proc SetPinFindConfNumber { } {
        global Params
        global _pin
        global _pinNotFilledCount
        
        set _pin [infotag get evt_dcdigits]
        if { ![info exists _pin] || $_pin == "" } {
                set _pinNotFilledCount [expr {$_pinNotFilledCount + 1}]
                PrintDebug "Pin wasn't entered for the $_pinNotFilledCount. time."
                if { $_pinNotFilledCount >= $Params(MaxPinNotFilledCount) } {
                        leg disconnect leg_incoming
                        return
                }
                RequestPin
                fsm setstate PinEntered
                return
        }
        PrintDebug "SetPinFindConfNumber '$_pin'"
        SendConfNumberRequest true
}

# *********************************
# SendConfNumberRequest
# *********************************
proc SendConfNumberRequest { isFirstRequest } {
        global _pin
        global _phoneNumber
        global Params
        global isHttpCallbackConfNumber
        
        set _phoneNumber [infotag get leg_ani]
        
        set isHttpCallbackConfNumber 0
        if { $isFirstRequest == "true" } {
                set url $Params(ServerUrl1)
                append url $Params(PageUrl)
                append url "?parameter=" $_pin
                append url "&phoneNumber=" $_phoneNumber
                ::httpios::geturl $url -command HttpCallbackConfNumber -timeout $Params(HttpTimeout)
        } else { 
                set isHttpCallbackConfNumber 1
                set url $Params(ServerUrl2)
                append url $Params(PageUrl)
                append url "?parameter=" $_pin
                append url "&phoneNumber=" $_phoneNumber
                ::httpios::geturl $url -command HttpCallbackConfNumber -timeout $Params(HttpTimeout)
        }
}

# *********************************
# HttpCallbackConfNumber
# *********************************
proc HttpCallbackConfNumber {token} {
        global isHttpCallbackConfNumber
        global _conferenceNumber
        PrintDebug "HttpCallbackConfNumber, isSecondCallback='$isHttpCallbackConfNumber'"

        PrintDebug "Status: '[::httpios::status $token]', ResponseCode: '[::httpios::ncode $token]', Data: '[::httpios::data $token]'"          
        if { [::httpios::ncode $token] == "200" } {
                PrintDebug "HttpCallbackConfNumber: ResponseCode is 200"
                set _conferenceNumber [::httpios::data $token] 
                SetConfNumbNoteTrans $_conferenceNumber
        } else {
                PrintDebug "HttpCallbackConfNumber: Error ResponseCode is '[::httpios::ncode $token]'"
                if { $isHttpCallbackConfNumber == 0 } {
                        set isHttpCallbackConfNumber 1
                        SendConfNumberRequest false
                } else {
                        PrintDebug "SendConfNumberRequest Failed twice"
                        PlayError
                        fsm setstate ErrorNotifi
                }
        } 
        ::httpios::cleanup $token
}

# *********************************
# SetConfNumbNoteTrans
# *********************************
proc SetConfNumbNoteTrans { _conferenceNumber } {
        global _prompts
        global _language
        global _collectDigitsParams
        
        PrintDebug "SetConfNumbNoteTrans '$_conferenceNumber'"
        
        if { $_conferenceNumber == ""} {
                PrintDebug "_conferenceNumber param is empty. Error"
                fsm setstate ErrorNotifi
                return
        }
        
        if { $_conferenceNumber == "error" || $_conferenceNumber == "" } {
                CheckBlockedNumberStep2
        } elseif { $_conferenceNumber == "notOpenYet"} {
            PlayConfNotOpenYet
        } elseif { $_conferenceNumber == "full"} {
                fsm setstate ConferenceFull
                NoteTransferToHelpdesk
        } else {
                if  { $_language == 2 } {
                        media play leg_incoming $_prompts(transfering_en)
                } else {
                        media play leg_incoming $_prompts(transfering_cz)
                }
        }
}

# *********************************
# PlayConfNotFound
# *********************************
proc PlayConfNotFound { } {
        global _prompts
        global _language
        global _collectDigitsParams
                
        PrintDebug "PlayConfNotFound"
        if { $_language == 2 } {
                media play leg_incoming $_prompts(confNotFound_en)
        } else {
                media play leg_incoming $_prompts(confNotFound_cz)
        }
        set _pinNotFilledCount 0
        fsm setstate PinEntered
        leg collectdigits leg_incoming _collectDigitsParams
}

# *********************************
# CheckBlockedNumberStep2
# *********************************
proc CheckBlockedNumberStep2 { } {

        CheckBlockedNumber 0
        #fsm setstate CheckBlockNumberStep2
}

# *********************************
# HttpCallbackBlockingSecondStep
# *********************************
proc HttpCallbackBlockingSecondStep {token} {
        global isHttpCallbackBlockingSecondStep
        global isSuccessResponseBlockingSecondStep
        incr isHttpCallbackBlockingSecondStep
        
        PrintDebug "HttpCallbackBlockingSecondStep, CallbackCount='$isHttpCallbackBlockingSecondStep', isSuccessResponseBlockingSecondStep='$isSuccessResponseBlockingSecondStep' "
        PrintDebug "Status: '[::httpios::status $token]', ResponseCode: '[::httpios::ncode $token]', Data: '[::httpios::data $token]'"          
        
        if { [::httpios::ncode $token] == "200" } {
                if { $isSuccessResponseBlockingSecondStep == "false" } {        
                        set isSuccessResponseBlockingSecondStep "true"
                        CheckBlockPlayMsg [::httpios::data $token]
                } else { 
                        PrintDebug "HttpCallbackBlockingSecondStep: Ignore second callback"
                }
        } else {
                if { $isHttpCallbackBlockingSecondStep == 2 && $isSuccessResponseBlockingSecondStep == "false" } {
                        PlayError
                        fsm setstate ErrorNotifi
                } else {
                        PrintDebug "HttpCallbackBlockingSecondStep: Ignore this callback"
                }
        }

        ::httpios::cleanup $token
}

# *********************************
# CheckBlockPlayMsg
# *********************************
proc CheckBlockPlayMsg { isBlacklisted } {

# if result 0 -> PlayConfNotFound
# else -> play error and set state NumberIsBlocked

        global _prompts
        global _language
        global _collectDigitsParams
        
        PrintDebug "CheckBlockPlayMsg '$isBlacklisted'"
        
        if { $isBlacklisted == "" } {
                PrintDebug "isBlacklisted param is empty. Error"
                fsm setstate ErrorNotifi
                return
        }
        
        if { $isBlacklisted == "NotBlacklisted"  } {
                PlayConfNotFound
        } else {
                if  { $_language == 2 } {
                        media play leg_incoming $_prompts(numberIsBlocked_en)
                        fsm setstate NumberIsBlocked
                } else {
                        media play leg_incoming $_prompts(numberIsBlocked_cz)
                        fsm setstate NumberIsBlocked
                }
        }
}

# *********************************
# PlayConfNotOpenYet
# *********************************
proc PlayConfNotOpenYet { } {
        global _prompts
        global _language
        global _collectDigitsParams
        
        if  { $_language == 2 } {
                media play leg_incoming $_prompts(confNotOpenYet_en)
        } else {
                media play leg_incoming $_prompts(confNotOpenYet_cz)
        }
        set _pinNotFilledCount 0
        fsm setstate PinEntered
        leg collectdigits leg_incoming _collectDigitsParams
}


# *********************************
# NoteTransferToHelpdesk
# *********************************
proc NoteTransferToHelpdesk { } {
        global _prompts
        global _language
        
        if  { $_language == 2 } {
                media play leg_incoming $_prompts(confFull_en)
        } else {
                media play leg_incoming $_prompts(confFull_cz)
        }
}

# *********************************
# TransferToConf
# *********************************
proc TransferToConf { } {
        global _conferenceNumber
        global Params

        set callInfo(mode) REDIRECT_ROTARY
        set callInfo(rerouteMode) REDIRECT_ROTARY
        
        PrintDebug "TransferToConf '$Params(PhonePrefix)$_conferenceNumber'"
        leg setup $Params(PhonePrefix)$_conferenceNumber callinfo leg_incoming
        PrintDebug "TransferToConf"
}

# *********************************
# SaveAttendeeDisconnected
# *********************************
proc SaveAttendeeDisconnected { } {
        PrintDebug "SaveAttendeeDisconnected"
        UpdateConferenceAttendeeConnected false true
}

# *********************************
# PlayError
# *********************************
proc PlayError { } {
        global _prompts

        PrintDebug "PlayError"
        media play leg_incoming $_prompts(transferError_all)
}

# *********************************
# UpdateConferenceAttendeeConnected
# *********************************
proc UpdateConferenceAttendeeConnected { isConnected isFirstRequest } {
        global Params
        global _conferenceNumber
        global _pin
        global _attendeeId
        global isHttpCallbackAttendeeConnected
        global isHttpCallbackAttendeeDisconnected
        
        if { ![info exists _conferenceNumber] || ![info exists _pin] } {
                PrintDebug "Conference is unknown."
                return
        }
        
        set ani [infotag get leg_ani ]
        if { $isConnected == "true" } {
                set attendeePageUrl $Params(AttendeeConnectedPageUrl)
                set attendeeId "none"
        } else {
                set attendeePageUrl $Params(AttendeeDisconnectedPageUrl)
                if { ![info exists _attendeeId]} {
                        PrintDebug "AttendeeId is unknown."
                        return
                }
                set attendeeId $_attendeeId
        }
        
        PrintDebug "UpdateConferenceAttendeeConnected: ani='$ani', isConnected='$isConnected', conferenceNumber='$_conferenceNumber', pin='$_pin', attendeeId='$attendeeId'"

        set isHttpCallbackAttendeeConnected 0
        set isHttpCallbackAttendeeDisconnected 0
        if { $isFirstRequest == "true" } {
                set url $Params(ServerUrl1)
                append url $attendeePageUrl
                append url "?phone=" $ani
                append url "&pin=" $_pin
                append url "&attendeeid=" $attendeeId
                if { $isConnected == "true" } {
                        ::httpios::geturl $url -command HttpCallbackAttendeeConnected -timeout $Params(HttpTimeout)
                } else {
                        ::httpios::geturl $url -command HttpCallbackAttendeeDisconnected -timeout $Params(HttpTimeout)
                }
        } else {
                set isHttpCallbackAttendeeConnected 1
                set isHttpCallbackAttendeeDisconnected 1
                set url $Params(ServerUrl2)
                append url $attendeePageUrl
                append url "?phone=" $ani
                append url "&pin=" $_pin
                append url "&attendeeid=" $attendeeId
                if { $isConnected == "true" } {
                        ::httpios::geturl $url -command HttpCallbackAttendeeConnected -timeout $Params(HttpTimeout)
                } else {
                        ::httpios::geturl $url -command HttpCallbackAttendeeDisconnected -timeout $Params(HttpTimeout)
                }
        }
}

# *********************************
# HttpCallbackAttendeeConnected
# *********************************
proc HttpCallbackAttendeeConnected {token} {
        global isHttpCallbackAttendeeConnected
        PrintDebug "HttpCallbackAttendeeConnected, isSecondCallback='$isHttpCallbackAttendeeConnected'"
        
        PrintDebug "Status: '[::httpios::status $token]', ResponseCode: '[::httpios::ncode $token]', Data: '[::httpios::data $token]'"          
        if { [::httpios::ncode $token] == "200" } {
                PrintDebug "HttpCallbackAttendeeConnected: ResponseCode is 200"
                SaveAttendeeId [::httpios::data $token]
        } else {
                PrintDebug "HttpCallbackAttendeeConnected: Error status is '[::httpios::status $token]'"
                if { $isHttpCallbackAttendeeConnected == 0 } {
                        set isHttpCallbackAttendeeConnected 1
                        UpdateConferenceAttendeeConnected true false
                } else {
                        PrintDebug "UpdateAttendeeConnected Failed twice"
                }
        } 

        ::httpios::cleanup $token
}

# *********************************
# HttpCallbackAttendeeDisconnected
# *********************************
proc HttpCallbackAttendeeDisconnected {token} {
        global isHttpCallbackAttendeeDisconnected
        PrintDebug "HttpCallbackAttendeeDisconnected, isSecondCallback='$isHttpCallbackAttendeeDisconnected'"
        
        PrintDebug "Status: '[::httpios::status $token]', ResponseCode: '[::httpios::ncode $token]', Data: '[::httpios::data $token]'"          
        if { [::httpios::ncode $token] == "200" } {
                PrintDebug "HttpCallbackAttendeeDisconnected: ResponseCode is 200"
                #CheckResponsePlayError [::httpios::data $token]
                CloseCall
        } else {
                PrintDebug "HttpCallbackAttendeeDisconnected: Error status is '[::httpios::status $token]'"
                if { $isHttpCallbackAttendeeDisconnected == 0 } {
                        set isHttpCallbackAttendeeDisconnected 1
                        UpdateConferenceAttendeeConnected false false
                } else {
                        PrintDebug "UpdateAttendeeDisconnected Failed twice"
                        CloseCall
                }
        }
        ::httpios::cleanup $token
}

# *********************************
# SaveAttendeeId
# *********************************
proc SaveAttendeeId { Parameters } {
        PrintDebug "SaveAttendeeId has been called"
        global _attendeeId
        global _prompts
        global _language

        set ParametersArr [split $Parameters ";"]
        set _attendeeId [lindex $ParametersArr 0]
        set _attendeeCount [lindex $ParametersArr 1]
        
        PrintDebug "Response: Attendee ID: '$_attendeeId'"
        PrintDebug "Response: Attendee Count: '$_attendeeCount'"
        
        if { $_attendeeId == "" } {
                PrintDebug "Attendee ID is null or empty."
                return
        }
        
        if { $_attendeeCount == 1 } {
                PrintDebug "Attendee is first"
                if  { $_language == 2 } {
                        media play leg_incoming $_prompts(conFirstAttendee_en) 
                } else {
                        media play leg_incoming $_prompts(conFirstAttendee_cz) 
                }
        } else {
                PrintDebug "Attendee isn't first !!!"
                TransferToConf
                fsm setstate TransferredToConf
        }
}

# *********************************
# TransferToHelpdesk
# *********************************
proc TransferToHelpdesk { } {
        global Params

        set callInfo(mode) REDIRECT_ROTARY
        set callInfo(rerouteMode) REDIRECT_ROTARY
        
        PrintDebug "TransferToHelpdesk '$Params(HelpdeskNumber)'"
        leg setup $Params(HelpdeskNumber) callinfo leg_incoming
        PrintDebug "TransferToHelpdesk"
}

# *********************************
# CheckTransToHelpdesk
# *********************************
proc CheckTransToHelpdesk { } {
        set setupStatus [infotag get evt_status]
        if  { $setupStatus == "ls_000"} {
                PrintDebug "CheckTransToHelpdesk connected"
        } else {
                PrintDebug "CheckTransToHelpdesk not connected: $setupStatus"
                NoteTransferToHelpdesk
        }
}

# ********************************* 
# SaveAttendeeToConnect
# *********************************
proc SaveAttendeeToConnect { } {
        PrintDebug "SaveAttendeeToConnect"
        UpdateConferenceAttendeeConnected true true
}

# *********************************
# CheckSetupResult
# *********************************
proc CheckSetupResult { } {
        global _prompts

        set ProcessSetupDone_setupStatus [infotag get evt_status]

        PrintDebug "CheckSetupResult '$ProcessSetupDone_setupStatus'"
        if  { $ProcessSetupDone_setupStatus == "ls_000"} {
                #connection create leg_incoming leg_outgoing
        } else {
                UpdateConferenceAttendeeConnected false true
                media play leg_incoming $_prompts(transferError_all)
                #fsm setstate AttendeeDisconnectedSavedError
        }
}

# *********************************
# StopTimerCloseCall
# *********************************
proc StopTimerCloseCall { } {
        PrintDebug "StopTimerCloseCall"
        StopTimer
        Disconnect
}

# *********************************
# Disconnect
# *********************************
proc Disconnect { } {
        PrintDebug "Disconnect"
        leg disconnect leg_incoming
}

# *********************************
# DisconnectAndSaveAttendeeDisconnected
# *********************************
proc DisconnectAndSaveAttendeeDisconnected { } {
        PrintDebug "DisconnectAndSaveAttendeeDisconnected"
        SaveAttendeeDisconnected
        Disconnect
}

# *********************************
# StopTimer
# *********************************
proc StopTimer { } {
        PrintDebug "StopTimer"
        timer stop named_timer timeout
}

# *********************************
# Application init
# *********************************
proc AppInit { } {
        param register Debug "To Enabled debug information" "false" "b"
        param register PhonePrefix "Conference number prefix" "9567"
        param register ServerUrl1 "URL to reach conference WebApp" "http://10.144.252.64/TeleKonference/"
        param register ServerUrl2 "Backup URL to reach conference WebApp" "http://10.144.252.64/TeleKonference/"
        param register PageUrl "WebApp page" "ConferenceNumber.aspx"
        param register AttendeeConnectedPageUrl "Web page that saves connected attendee." "Tcl/ConferenceAttendeeConnected.aspx"
        param register AttendeeDisconnectedPageUrl "Web page that saves disconnected attendee." "Tcl/ConferenceAttendeeDisconnected.aspx"
        param register MaxLanguageNotFilledCount "Max number of requests to select language." "2"
        param register MaxPinNotFilledCount "Max number of requests to select language." "3"
        param register DigitCollectTimeout "Time in seconds the application waits for a DTMF digit." "15"
        param register HelpdeskNumber "Phone number of helpdesk." "1811"
        param register HttpTimeout "HTTP request timeout in seconds." "2"
}

# *********************************
# GetParamString
# *********************************
proc GetParamString { paramName default } {
        global Params
        
        if {[infotag get cfg_avpair_exists $paramName]} {
                set Params($paramName) [string trim [infotag get cfg_avpair $paramName]]
                        PrintDebug "GetParamString: $paramName - $Params($paramName)"
        } else {
                set Params($paramName) $default
        }
}

# *********************************
# GetParamInt
# *********************************
proc GetParamInt { paramName default } {
        global Params
        
        if {[infotag get cfg_avpair_exists $paramName]} {
                set Params($paramName) [string trim [infotag get cfg_avpair $paramName]]
                PrintDebug "GetParamInt: $paramName - $Params($paramName)"
        } else {
                set Params($paramName) $default
        }
}

# *********************************
# GetParamBool
# *********************************
proc GetParamBool { paramName default } {
        global Params
        
        if {[infotag get cfg_avpair_exists $paramName]} {
                set tmp [string tolower [string trim [infotag get cfg_avpair $paramName]]]
                switch $tmp {
                        true {
                                set Params($paramName) true
                        }
                        1 {
                                set Params($paramName) true
                        }
                        false {
                                set Params($paramName) false
                        }
                        0 {
                                set Params($paramName) false
                        }
                        default {
                                PrintDebug "Error in reading parameters: $paramName"
                                set Params($paramName) $default
                        }
                }
                PrintDebug "GetParamBool: $paramName - $Params($paramName)"
        } else {
                set Params($paramName) $default
        }
}

# *********************************
# Print debug
# *********************************
proc PrintDebug { text } {
        global Params

        if {$Params(Debug) == "true"} {
                puts "--==** $text **==--"
        }
}

AppInit

requiredversion 2.0
package require httpios 1.0
# *********************************
# State Machine
# *********************************
set fsm(INIT_STATE,ev_setup_indication)                                         "CallInitReqLanguage                            LanguageSelected"
set fsm(LanguageSelected,ev_collectdigits_done)                                 "SetLanguageCheckBlockedNumber                  PinEntered"
set fsm(PinEntered,ev_collectdigits_done)                                       "SetPinFindConfNumber                           TransferNotified"
set fsm(TransferNotified,ev_media_done)                                         "SaveAttendeeToConnect                          FirstTransferNotified"
set fsm(FirstTransferNotified,ev_media_done)                                    "TransferToConf                                 TransferredToConf"
set fsm(TransferredToConf,ev_setup_done)                                        "CheckSetupResult                               same_state"
set fsm(ErrorNotifi,ev_media_done)                                              "Disconnect                                     same_state"
set fsm(ConferenceFull,ev_media_done)                                           "TransferToHelpdesk                             same_state"
set fsm(ConferenceFull,ev_setup_done)                                           "CheckTransToHelpdesk                           same_state"
set fsm(any_state,ev_disconnected)                                              "SaveAttendeeDisconnected                       AttendeeDisconnectedSaved"
set fsm(NumberIsBlocked,ev_media_done)                                          "DisconnectAndSaveAttendeeDisconnected          AttendeeDisconnectedSaved"


fsm define fsm INIT_STATE