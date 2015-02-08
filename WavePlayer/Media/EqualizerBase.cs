using System;
using System.Collections.Generic;
using WavePlayer.Localization;

namespace WavePlayer.Media
{
    public abstract class EqualizerBase : IEqualizer
    {
        private EqualizerPreset _currentPreset;

        public event EventHandler PresetChanged;

        public virtual bool IsEnabled { get; set; }

        public float MaximumGain { get { return 12.0f; } }

        public float MinimumGain { get { return -12.0f; } }

        public IEnumerable<int> FrequencyRange
        {
            get { return EqualizerPreset.DefaultPresets[EqualizerPreset.Default].Bands.Keys; }
        }

        public EqualizerPreset CurrentPreset
        {
            get { return _currentPreset; }

            set
            {
                if (_currentPreset == value || value == null)
                {
                    return;
                }

                _currentPreset = value;

                SetPresetBands(_currentPreset);

                var handler = PresetChanged;

                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        public void Initialize()
        {
            var preset = EqualizerPreset.DefaultPresets[EqualizerPreset.Default];

            InitializeInternal(true, preset.Bands);

            _currentPreset = preset;
        }

        public void Reset()
        {
            if (CurrentPreset.Name != EqualizerPreset.Manual)
            {
                return;
            }

            var defaultPreset = EqualizerPreset.DefaultPresets[EqualizerPreset.Default];

            SetManualPresetBands(defaultPreset.Bands);
        }

        public void SetBandGain(int frequency, float gain)
        {
            SetBandGainInternal(frequency, gain);

            SetManualPreset();

            SaveBandGain(frequency, gain);
        }

        public float GetBandGain(int frequency)
        {
            return GetBandGainInternal(frequency);
        }

        public void UpdateLocalization()
        {
            foreach (var preset in EqualizerPreset.DefaultPresets.Values)
            {
                preset.DisplayName = EqualizerPreset.GetDisplayName(preset.Name);
            }
        }

        protected abstract void SetBandGainInternal(int frequency, float gain);

        protected abstract float GetBandGainInternal(int frequency);

        protected abstract void InitializeInternal(bool isEnabled, IDictionary<int, float> bands);
        
        private void SetManualPreset()
        {
            var currentPreset = CurrentPreset;

            if (currentPreset.Name == EqualizerPreset.Manual)
            {
                return;
            }

            CurrentPreset = SetManualPresetBands(currentPreset.Bands);
        }

        private void SetPresetBands(EqualizerPreset currentPreset)
        {
            foreach (var band in currentPreset.Bands)
            {
                SetBandGainInternal(band.Key, band.Value);
            }
        }

        private static EqualizerPreset SetManualPresetBands(IDictionary<int, float> bands)
        {
            var manualPreset = EqualizerPreset.DefaultPresets[EqualizerPreset.Manual];

            foreach (var band in bands)
            {
                manualPreset.Bands[band.Key] = band.Value;
            }

            return manualPreset;
        }

        private void SaveBandGain(int frequency, float gain)
        {
            if (CurrentPreset.Name != EqualizerPreset.Manual)
            {
                return;
            }

            CurrentPreset.Bands[frequency] = gain;
        }
    }
}
