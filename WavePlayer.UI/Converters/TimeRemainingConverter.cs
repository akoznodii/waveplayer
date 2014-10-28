using System;
using System.Windows.Data;

namespace WavePlayer.UI.Converters
{
    public class TimeRemainingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is double) || !(values[1] is double))
            {
                return string.Empty;
            }

            var position = TimeSpan.FromMilliseconds((double)values[0]);
            var duration = TimeSpan.FromMilliseconds((double)values[1]);
            var value = duration > position ? duration - position : TimeSpan.Zero;
            return TimeSpanHelper.FormatTimeSpan(value, true);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
