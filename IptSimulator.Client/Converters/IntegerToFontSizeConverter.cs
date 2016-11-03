using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IptSimulator.Client.Converters
{
    public class IntegerToFontSizeConverter : IValueConverter
    {
        private static readonly FontSizeConverter Converter = new FontSizeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontSize = value as int?;
            if (!fontSize.HasValue) return value;
        
            return Converter.ConvertFromString(fontSize.Value + "pt");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
