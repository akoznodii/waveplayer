using System.Collections.Generic;

namespace WavePlayer.UI.Themes
{
    public interface IThemeService
    {
        Theme CurrentTheme { get; }

        Accent CurrentAccent { get; }
        
        IEnumerable<Accent> AvailableAccents { get; }

        IEnumerable<Theme> AvailableThemes { get; }

        void ChangeTheme(string newThemeName, string newAccentName, bool fallback = false);
    }
}
