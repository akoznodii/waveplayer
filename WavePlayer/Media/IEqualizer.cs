using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Media
{
    public interface IEqualizer
    {
        bool IsEnabled { get; set; }

        IEnumerable<int> FrequencyRange { get; } 

        void SetBandGain(int frequency, float gain);

        float GetBandGain(int frequency);
    }
}
