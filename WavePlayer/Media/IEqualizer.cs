using System;
using System.Collections.Generic;

namespace WavePlayer.Media
{
    public interface IEqualizer
    {
        event EventHandler PresetChanged;

        bool IsEnabled { get; set; }

        IEnumerable<int> FrequencyRange { get; }

        EqualizerPreset CurrentPreset { get; }

        void SetBandGain(int frequency, float gain);

        float GetBandGain(int frequency);

        void Reset();
    }
}
