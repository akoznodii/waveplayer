using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavePlayer.Fmod.Native;
using WavePlayer.Media;

namespace WavePlayer.Fmod
{
    internal sealed class Equalizer : IEqualizer, IDisposable
    {
        private readonly Dictionary<int, Dsp> _bands = new Dictionary<int, Dsp>();
        private readonly FmodSystem _system;
        private bool _isEnabled;

        public Equalizer(FmodSystem system)
        {
            _system = system;
        }

        public bool IsEnabled
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

        public void Initialize(bool isEnabled, IDictionary<int, float> frequencyGains)
        {
            var gainsSpecified = frequencyGains != null;

            _isEnabled = isEnabled;

            using (var channelGroup = _system.MasterChannelGroup)
            {
                foreach (var frequency in EqualizerHelper.FrequencyRange)
                {
                    var gain = gainsSpecified && frequencyGains.ContainsKey(frequency) ? frequencyGains[frequency] : 0;

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

        public IEnumerable<int> FrequencyRange
        {
            get
            {
                return EqualizerHelper.FrequencyRange;
            }
        } 

        public void SetBandGain(int frequency, float gain)
        {
            if (GetBandGain(frequency) == gain)
            {
                return;
            }

            Dsp dsp;

            if (_bands.TryGetValue(frequency, out dsp))
            {
                dsp.SetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_GAIN, gain);
                _system.Update();
            }
        }

        public float GetBandGain(int frequency)
        {
            Dsp dsp;

            if (_bands.TryGetValue(frequency, out dsp))
            {
                return dsp.GetParameter((int)EqualizerParameters.FMOD_DSP_PARAMEQ_GAIN);
            }

            return 0;
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
