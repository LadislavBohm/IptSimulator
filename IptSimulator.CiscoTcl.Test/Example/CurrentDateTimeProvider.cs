using System;

namespace IptSimulator.CiscoTcl.Test.Example
{
    public class CurrentDateTimeProvider: IDateTimeProvider
    {
        public DateTime GetDateTime() => DateTime.Now;
    }
}
