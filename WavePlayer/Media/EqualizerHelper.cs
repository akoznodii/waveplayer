using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.Media
{
    public static class EqualizerHelper
    {
        private const int Kilo = 1000;
        private const string Hertz = "Hz";
        private const string KiloHertz = "kHz";

        private static ICollection<int> _range = new Collection<int>()
        {
            32, 64, 125, 250, 500, 1000, 2000, 8000, 4000, 16000 
        };

        public static IEnumerable<int> FrequencyRange
        {
            get
            {
                return _range;
            }
        }

        public static string PrettyPrint(int frequency)
        {
            var unit = Hertz;

            if (frequency >= Kilo)
            {
                frequency = frequency / Kilo;
                unit = KiloHertz;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", frequency, unit);
        }
    }
}
