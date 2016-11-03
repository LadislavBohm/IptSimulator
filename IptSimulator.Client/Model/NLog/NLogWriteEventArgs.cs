using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace IptSimulator.Client.Model.NLog
{
    public class NLogWriteEventArgs : EventArgs
    {
        public NLogWriteEventArgs(LogEventInfo logEventInfo)
        {
            LogEventInfo = logEventInfo;
        }

        public LogEventInfo LogEventInfo { get; }
    }
}
