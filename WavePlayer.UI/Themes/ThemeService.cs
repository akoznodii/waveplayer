﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using WavePlayer.Configuration;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.Themes
{
    public class ThemeService : IThemeService
    {
        private const string DefaultAccent = "Blue";
        private readonly IConfigurationService _configurationService;
        private readonly Lazy<List<Accent>> _availableAccents = new Lazy<List<Accent>>(LoadAccents, true);
        private readonly Lazy<List<Theme>> _availableThemes = new Lazy<List<Theme>>(LoadThemes, true);

        public ThemeService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public Theme CurrentTheme
        {
            get; private set;
        }

        public Accent CurrentAccent
        {
            get; private set;
        }

        public IEnumerable<Accent> AvailableAccents
        {
            get { return _availableAccents.Value; }
        }

        public IEnumerable<Theme> AvailableThemes
        {
            get { return _availableThemes.Value; }
        }

        public void ChangeTheme(string newThemeName, string newAccentName, bool fallback = false)
        {
            var accent = ThemeManager.Accents
                                     .SingleOrDefault(a => a.Name == newAccentName);

            if (accent == null)
            {
                if (fallback)
                {
                    accent = ThemeManager.Accents.Single(a => a.Name == DefaultAccent);

                    Debug.WriteLine("Accent color: {0} was not found. Setup default accent color: {1}", newAccentName, accent.Name);
                }
                else
                {
                    var errorMessage = string.Format(Resources.Culture, Resources.AccentNotFound);

                    throw new InvalidOperationException(errorMessage);
                }
            }

            var theme = ThemeManager.AppThemes
                                     .SingleOrDefault(a => a.Name == newThemeName);

            if (theme == null)
            {
                if (fallback)
                {
                    theme = ThemeManager.AppThemes.First();
                    Debug.WriteLine("Background theme: {0} was not found. Setup default background theme: {1}", newThemeName, theme);
                }
                else
                {
                    var errorMessage = string.Format(Resources.Culture, Resources.ThemeNotFound);

                    throw new InvalidOperationException(errorMessage);
                }
            }

            ThemeManager.ChangeAppStyle(Application.Current, accent, theme);

            CurrentTheme = AvailableThemes.Single(a => a.Name == theme.Name);
            CurrentAccent = AvailableAccents.Single(a => a.Name == accent.Name);
            
            _configurationService.AccentColor = accent.Name;
            _configurationService.Theme = theme.Name;
        }

        private static List<Accent> LoadAccents()
        {
            return ThemeManager.Accents
                  .Select(GetAccent)
                  .ToList();
        }

        private static List<Theme> LoadThemes()
        {
            return ThemeManager.AppThemes
                  .Select(GetTheme)
                  .ToList();
        }

        private static Accent GetAccent(MahApps.Metro.Accent accent)
        {
            return new Accent() { Name = accent.Name, Color = (Color)accent.Resources["AccentColor"] };
        }

        private static Theme GetTheme(MahApps.Metro.AppTheme theme)
        {
            return new Theme() { Name = theme.Name };
        }
    }
}
