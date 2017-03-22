using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IptSimulator.Client.Converters
{
    public class DoubleMinusConstantConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = value as double?;
            if (!intValue.HasValue)
            {
                return value;
            }
            var paramInt = parameter as int?;
            if (!paramInt.HasValue)
            {
                return intValue.Value;
            }

            return intValue.Value - paramInt.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
