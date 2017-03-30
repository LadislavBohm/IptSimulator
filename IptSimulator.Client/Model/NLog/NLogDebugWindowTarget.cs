using System;
using NLog;
using NLog.Targets;

namespace IptSimulator.Client.Model.NLog
{
    [Target("DebugWindow")]
    public class NLogDebugWindowTarget : TargetWithLayout
    {
        protected override void Write(LogEventInfo logEvent)
        {
            RaiseLogReceivedEvent(logEvent);
        }

        private void RaiseLogReceivedEvent(LogEventInfo logEvent)
        {
            LogReceived?.Invoke(this,new NLogWriteEventArgs(logEvent));
        }

        public static event EventHandler<NLogWriteEventArgs> LogReceived; 
    }
}
