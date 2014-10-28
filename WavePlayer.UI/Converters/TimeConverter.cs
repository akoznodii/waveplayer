using System;
using System.Windows.Data;

namespace WavePlayer.UI.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                var timePosition = TimeSpan.FromMilliseconds((double)value);

                return TimeSpanHelper.FormatTimeSpan(timePosition);
            }
            
            if (value is TimeSpan)
            {
                return TimeSpanHelper.FormatTimeSpan((TimeSpan)value);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
