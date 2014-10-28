using System;
using WavePlayer.Common;

namespace WavePlayer.UI.Themes
{
    public class Theme : ModelBase
    {
        public string DisplayName { get { return Name; } }
        
        public string Name { get; set; }
    }
}
