using System;
using System.Collections.Generic;
using WavePlayer.Fmod.Native;
using WavePlayer.Media;

namespace WavePlayer.Fmod
{
    internal sealed class Equalizer : EqualizerBase, IDisposable
    {
        private readonly Dictionary<int, Dsp> _bands = new Dictionary<int, Dsp>();
        private readonly FmodSystem _system;
        private bool _isEnabled;

        public Equalizer(FmodSystem system)
        {
            _system = system;
        }

        public override bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                foreach (var band in _bands.Values)
                {
                    band.IsActive = value;
                }

                _system.Update();

                _isEnabled = value;
            }
        }

        protected override void InitializeInternal(bool isEnabled, IDictionary<int, float> bands)
        {
            var bandsSpecified = bands != null;

            _isEnabled = isEnabled;

            using (var channelGroup = _system.MasterChannelGroup)
            {
                foreach (var frequency in FrequencyRange)
                {
                    var gain = bandsSpecified && bands.ContainsKey(frequency) ? bands[frequency] : 0;

                    var dsp = _system.CreateDsp(DspType.FMOD_DSP_TYPE_PARAMEQ);

                    dsp.SetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_CENTER, frequency);
                    dsp.SetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_BANDWIDTH, 1);
                    dsp.SetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_GAIN, gain);
                    dsp.IsActive = isEnabled;

                    channelGroup.Add(dsp);

                    _bands.Add(frequency, dsp);
                }
            }

            _system.Update();
        }

        protected override void SetBandGainInternal(int frequency, float gain)
        {
            if (Math.Abs(GetBandGain(frequency) - gain) < Single.Epsilon)
            {
                return;
            }

            Dsp dsp;

            if (!_bands.TryGetValue(frequency, out dsp))
            {
                return;
            }

            dsp.SetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_GAIN, gain);
            _system.Update();
        }

        protected override float GetBandGainInternal(int frequency)
        {
            Dsp dsp;

            return _bands.TryGetValue(frequency, out dsp) ?
                dsp.GetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_GAIN) :
                0;
        }

        public void Dispose()
        {
            using (var channelGroup = _system.MasterChannelGroup)
            {
                foreach (var band in _bands.Values)
                {
                    channelGroup.Remove(band);

                    band.Dispose();
                }
            }

            _bands.Clear();
        }
    }
}
