using System;

namespace IptSimulator.CiscoTcl.Test.Example
{
    public class CustomDateTimeProvider: IDateTimeProvider
    {
        private readonly DateTime _customDate;

        public CustomDateTimeProvider(DateTime customDate)
        {
            _customDate = customDate;
        }

        public DateTime GetDateTime() => _customDate;
    }
}
