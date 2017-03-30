using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IptSimulator.CiscoTcl.Test.Example
{
    /// <summary>
    /// Just an example unit test class for thesis purposes.
    /// </summary>
    public class UnitTestExample
    {
        private readonly IDateTimeProvider _dateProvider
            = new CustomDateTimeProvider(new DateTime(2016, 1, 1));

        [Fact]
        public void GetTotalQuarterSum_MultipleOrders_FilteredTotalSum()
        {
            var date = new DateTime(2016,2,1);
            var totalSum = GetTotalQuarterSum(new[]
            {
                new ValueTuple<DateTime, decimal>(date.AddMonths(-1), 20),
                new ValueTuple<DateTime, decimal>(date.AddMonths(4), 30),
                new ValueTuple<DateTime, decimal>(date.AddMonths(-2), 80),
                new ValueTuple<DateTime, decimal>(date.AddMonths(1), 70),
                new ValueTuple<DateTime, decimal>(date.AddMonths(-6), 20),
                new ValueTuple<DateTime, decimal>(date, 10)
            });

            Assert.Equal(100, totalSum);
        }


        public decimal GetTotalQuarterSum(IEnumerable<(DateTime date, decimal totalPrice)> orders)
        {
            var date = _dateProvider.GetDateTime();
            var quarter = (date.Month - 1) / 3;

            return orders
                    .Where(o => 
                        o.date.Year == date.Year && 
                        (o.date.Month - 1) / 3 == quarter)
                    .Sum(o => o.totalPrice);
        }
    }
}
