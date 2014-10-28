using System;
using System.Windows.Data;

namespace WavePlayer.UI.Converters
{
    public class DoubleCompareConverter : IValueConverter
    {
        public const string Less = "Less";
        public const string Greater = "Greater";
        public const string Equal = "Equal";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double && parameter is double)
            {
                var compareValue = (double)parameter;
                var actualValue = (double)value;

                if (actualValue > compareValue)
                {
                    return Greater;
                }

                if (actualValue < compareValue)
                {
                    return Less;
                }

                return Equal;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
