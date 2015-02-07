using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WavePlayer.Common;

namespace WavePlayer.Media
{
    public class EqualizerPreset : ModelBase
    {
        public const string Default = "Default";
        public const string Manual = "Manual";

        private static readonly Lazy<IDictionary<string, EqualizerPreset>> DefaultPresetsInitializer = new Lazy<IDictionary<string, EqualizerPreset>>(CreateDefaultPresets);

        private static readonly ICollection<string> DefaultPresetNames = new Collection<string>()
        {
           Default, Manual,
        };

        private static readonly ICollection<int> FrequencyRange = new Collection<int>()
        {
            32, 64, 125, 250, 500, 1000, 2000, 8000, 4000, 16000 
        };

        private string _displayName;

        public EqualizerPreset(string name, IDictionary<int, float> bands)
        {
            Name = name;
            Bands = bands;
        }

        public string Name { get; private set; }

        public string DisplayName { get { return _displayName; } set { SetField(ref _displayName, value); } }

        public IDictionary<int, float> Bands { get; private set; }

        public static IDictionary<string, EqualizerPreset> DefaultPresets { get { return DefaultPresetsInitializer.Value; } }

        public static string GetDisplayName(string name)
        {
            switch (name)
            {
                case Default:
                    return "Default";
                case Manual:
                    return "Manual";
                default:
                    return String.Empty;
            }
        }

        private static IDictionary<string, EqualizerPreset> CreateDefaultPresets()
        {
            var presets = new Dictionary<string, EqualizerPreset>();
            
            foreach (var presetName in DefaultPresetNames)
            {
                var name = GetDisplayName(presetName);

                if (String.IsNullOrEmpty(name)) { continue; }

                var preset = CreatePreset(presetName);

                preset.DisplayName = name;

                presets.Add(presetName, preset);
            }

            return presets;
        }

        private static EqualizerPreset CreatePreset(string presetName)
        {
            IDictionary<int, float> bands;

            switch (presetName)
            {
                case Default:
                case Manual:
                    bands = CreateBands();
                    break;
                default:
                    throw new ArgumentNullException("bands");
            }

            return new EqualizerPreset(presetName, bands);
        }

        private static IDictionary<int, float> CreateBands(params float[] gains)
        {
            var idx = 0;
            var result = new Dictionary<int, float>();

            foreach (var frequency in FrequencyRange)
            {
                var gain = gains != null && gains.Length < idx ? gains[idx] : 0;
                result.Add(frequency, gain);
                idx++;
            }

            return result;
        }
    }
}
