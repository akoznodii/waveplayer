using System.Windows.Data;
using VK.Groups;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.Converters
{
    public class GroupTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is GroupType)
            {
                var type = (GroupType)value;

                switch (type)
                {
                   case GroupType.Event:
                        return Resources.Event;
                   case GroupType.Group:
                        return Resources.Group;
                   case GroupType.Page:
                        return Resources.Page;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
