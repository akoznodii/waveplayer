using System;
using System.Globalization;

namespace WavePlayer.UI.Converters
{
    internal static class TimeSpanHelper
    {
        public static string FormatTimeSpan(TimeSpan value, bool timeRemainingFormat = false)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", timeRemainingFormat ? "-" : string.Empty, value.Hours > 0 ? value.ToString(@"hh\:m\:ss", CultureInfo.InvariantCulture) : value.ToString(@"m\:ss", CultureInfo.InvariantCulture));
        }
    }
}
