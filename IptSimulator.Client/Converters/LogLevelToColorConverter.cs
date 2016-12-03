using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using NLog;

namespace IptSimulator.Client.Converters
{
    public class LogLevelToColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush DebugColor = new SolidColorBrush(Colors.WhiteSmoke);
        private static readonly SolidColorBrush InfoColor = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush WarningColor = new SolidColorBrush(Color.FromArgb(157, 243, 225, 14));
        private static readonly SolidColorBrush ErrorColor = new SolidColorBrush(Color.FromArgb(142, 238, 0, 0));
        private static readonly IDictionary<string,SolidColorBrush> LevelColorDictionary = new Dictionary<string, SolidColorBrush>()
        {
            { "DEBUG", DebugColor },
            { "INFO", InfoColor },
            { "WARN", WarningColor },
            { "ERROR", ErrorColor }
        }; 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (value as string ?? string.Empty).ToUpper();

            SolidColorBrush result;
            if(!LevelColorDictionary.TryGetValue(level, out result))
            {
                result = InfoColor;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
