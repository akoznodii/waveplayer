using System.Collections.Generic;
using WavePlayer.Media;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal class Equalizer : EqualizerBase
    {
        protected override void SetBandGainInternal(int frequency, float gain)
        {
        }

        protected override float GetBandGainInternal(int frequency)
        {
            return 0;
        }

        protected override void InitializeInternal(bool isEnabled, IDictionary<int, float> bands)
        {
        }
    }
}

#endif
