using System;
using System.Collections.Generic;
using System.Globalization;

namespace WavePlayer.Localization
{
    public interface ILocalizationService
    {
        event EventHandler<EventArgs> CurrentCultureChanged;

        CultureInfo CurrentCulture { get; }

        IEnumerable<CultureInfo> AvailableCultures { get; }

        void SetCurrentCulture(int lcid, bool fallback = false);

        void RegisterResource(Type resourceType);
    }
}
