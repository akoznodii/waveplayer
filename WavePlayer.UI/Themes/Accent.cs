using System.Windows.Media;
using WavePlayer.Common;

namespace WavePlayer.UI.Themes
{
    public class Accent : ModelBase
    {
        public string DisplayName { get { return Name; } }
        
        public string Name { get; set; }

        public Color Color { get; set; }
    }
}
