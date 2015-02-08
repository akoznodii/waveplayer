using System;
using System.Collections.Generic;
using WavePlayer.Localization;

namespace WavePlayer.Media
{
    public interface IEqualizer : ILocalizable
    {
        event EventHandler PresetChanged;

        float MaximumGain { get; }

        float MinimumGain { get; }

        bool IsEnabled { get; set; }

        IEnumerable<int> FrequencyRange { get; }

        EqualizerPreset CurrentPreset { get; set; }

        void SetBandGain(int frequency, float gain);

        float GetBandGain(int frequency);

        void Reset();
    }
}
