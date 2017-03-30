using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Events
{
    public class CiscoTclEvents
    {
        public const string AccountingStatusInd = "ev_accounting_status_ind";
        public const string AddressResolved = "ev_address_resolved";
        public const string Alert = "ev_alert";
        public const string AnyEvent = "ev_any_event";
        public const string AuthorizeDone = "ev_authorize_done";
        public const string AuthenticateDone = "ev_authenticate_done";
        public const string CallTimer = "ev_call_timer0";
        public const string CollectDigitsDone = "ev_collectdigits_done";
        public const string Connected = "ev_connected";
        public const string ConsultRequest = "ev_consult_request";
        public const string ConsultResponse = "ev_consult_response";
        public const string ConsultationDone = "ev_consultation_done";
        public const string CreateDone = "ev_create_done";
        public const string DestroyDone = "ev_destroy_done";
        public const string DigitEnd = "ev_digit_end";
        public const string DisconnectDone = "ev_disconnect_done";
        public const string Disconnected = "ev_disconnected";
        public const string DiscProgInd = "ev_disc_prog_ind";
        public const string Facility = "ev_facility";
        public const string Feature = "ev_feature";
        public const string Grab = "ev_grab";
        public const string HookFlash = "ev_hookflash";
        public const string Handoff = "ev_handoff";
        public const string LegTimer = "ev_leg_timer";
        public const string MediaActivity = "ev_media_activity";
        public const string MediaDone = "ev_media_done";
        public const string MediaInactivity = "ev_media_inactivity";
        public const string NamedTimer = "ev_named_timer";
        public const string Proceeding = "ev_proceeding";
        public const string Progress = "ev_progress";
        public const string Returned = "ev_returned";
        public const string SetupDone = "ev_setup_done";
        public const string SetupIndication = "ev_setup_indication";
        public const string Synthesizer = "ev_synthesizer";
        public const string ToneDetected = "ev_tone_detected";
        public const string TransferRequest = "ev_transfer_request";
        public const string TransferStatus = "ev_transfer_status";
        public const string VxmldialogDone = "ev_vxmldialog_done";
        public const string VxmldialogEvent = "ev_vxmldialog_event";
        public const string MsgIndication = "ev_msg_indication";
        public const string SessionIndication = "ev_session_indication";
        public const string SessionTerminate = "ev_session_terminate";
        public const string SubscribeDone = "ev_subscribe_done";
        public const string UnsubscribeDone = "ev_unsubscribe_done";
        public const string UnsubscribeIndication = "ev_unsubscribe_indication";
        public const string Notify = "ev_notify";
        public const string NotifyDone = "ev_notify_done";
        public const string SubscribeCleanup = "ev_subscribe_cleanup";

        public static IReadOnlyCollection<string> All { get; } = new List<string>
        {
            AccountingStatusInd,AddressResolved, Alert, AnyEvent, AuthorizeDone, AuthenticateDone, CallTimer, CollectDigitsDone, Connected, ConsultRequest,
            ConsultResponse, ConsultationDone, CreateDone, DestroyDone, DigitEnd, DisconnectDone, Disconnected, DiscProgInd, Facility, Feature, Grab, HookFlash,
            Handoff, LegTimer, MediaActivity, MediaDone, MediaInactivity, NamedTimer, Proceeding, Progress, Returned, SetupDone, SetupIndication, Synthesizer,
            ToneDetected, TransferRequest, TransferStatus, VxmldialogDone, VxmldialogEvent, MsgIndication, SessionIndication, SessionTerminate, SubscribeDone,
            UnsubscribeDone, UnsubscribeIndication, Notify, NotifyDone, SubscribeCleanup
        };
    }
}
