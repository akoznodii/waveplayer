﻿using System.Globalization;
using WavePlayer.Media;

namespace WavePlayer.UI.ViewModels
{
    public class EqualizerBandViewModel : ViewModelBase
    {
        private readonly IEqualizer _equalizer;
        private readonly int _frequency;

        public EqualizerBandViewModel(int frequency, IEqualizer equalizer)
        {
            _frequency = frequency;
            _equalizer = equalizer;

            Frequency = ConvertFrequency(_frequency);
        }

        public string Frequency { get; private set; }

        public float MaximumGain { get { return _equalizer.MaximumGain; } }

        public float MinimumGain { get { return _equalizer.MinimumGain; } }

        public float Gain
        {
            get { return _equalizer.GetBandGain(_frequency); }
            set { _equalizer.SetBandGain(_frequency, value); }
        }

        public void Refresh()
        {
            RaisePropertyChanged("Gain");
        }

        private static string ConvertFrequency(int frequency)
        {
            var preffix = string.Empty;

            if (frequency >= 1000)
            {
                frequency = frequency / 1000;

                preffix = "K";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:D}{1}", frequency, preffix);
        }
    }
}