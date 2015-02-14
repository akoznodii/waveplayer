using System.Collections.Generic;
using WavePlayer.Media;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal class Equalizer : EqualizerBase
    {
        private readonly IDictionary<int, float> _bands = new Dictionary<int, float>();

        protected override void SetBandGainInternal(int frequency, float gain)
        {
            _bands[frequency] = gain;
        }

        protected override float GetBandGainInternal(int frequency)
        {
            return _bands[frequency];
        }

        protected override void InitializeInternal(bool isEnabled, IDictionary<int, float> bands)
        {
            foreach (var band in bands)
            {
                _bands.Add(band.Key, band.Value);
            }
        }
    }
}

#endif
