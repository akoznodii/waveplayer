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
        public const string BassBoost = "BassBoost";
        public const string Loudness = "Loudness";
        public const string Headphones = "Headphones";
        public const string Acoustic = "Acoustic";
        public const string Classical = "Classical";
        public const string Dance = "Dance";
        public const string Club = "Club";
        public const string HipHop = "HipHop";
        public const string Jazz = "Jazz";
        public const string Pop = "Pop";
        public const string RhythmAndBlues = "RhythmAndBlues";
        public const string Rock = "Rock";
        public const string Party = "Party";
        public const string Ska = "Ska";
        public const string Reggae = "Reggae";
        public const string Techno = "Techno";
        public const string Live = "Live";
        public const string Speech = "Speech";

        private static readonly Lazy<IDictionary<string, EqualizerPreset>> DefaultPresetsInitializer = new Lazy<IDictionary<string, EqualizerPreset>>(CreateDefaultPresets);

        private static readonly ICollection<string> DefaultPresetNames = new Collection<string>()
        {
           Default, Manual, BassBoost, Loudness, Headphones, Acoustic, Classical, Dance, Club, Techno, HipHop, Jazz, Pop, RhythmAndBlues, Rock,
           Party, Ska, Reggae, Live, Speech
        };

        private static readonly ICollection<int> FrequencyRange = new Collection<int>()
        {
            32, 64, 125, 250, 500, 1000, 200, 4000, 8000, 16000 
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
                    return WavePlayerLocalization.Default;
                case Manual:
                    return WavePlayerLocalization.Manual;
                case BassBoost:
                    return WavePlayerLocalization.BassBoost;
                case Loudness:
                    return WavePlayerLocalization.Loudness;
                case Acoustic:
                    return WavePlayerLocalization.Acoustic;
                case Classical:
                    return WavePlayerLocalization.Classical;
                case Dance:
                    return WavePlayerLocalization.Dance;
                case Club:
                    return WavePlayerLocalization.Electronic;
                case HipHop:
                    return WavePlayerLocalization.HipHop;
                case Jazz:
                    return WavePlayerLocalization.Jazz;
                case Pop:
                    return WavePlayerLocalization.Pop;
                case Rock:
                    return WavePlayerLocalization.Rock;
                case RhythmAndBlues:
                    return WavePlayerLocalization.RhythmAndBlues;
                case Speech:
                    return WavePlayerLocalization.Speech;
                case Headphones:
                    return WavePlayerLocalization.Headphones;
                case Party:
                    return WavePlayerLocalization.Party;
                case Ska:
                    return WavePlayerLocalization.Ska;
                case Reggae:
                    return WavePlayerLocalization.Reggae;
                case Techno:
                    return WavePlayerLocalization.Techno;
                case Live:
                    return WavePlayerLocalization.Live;
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
                case BassBoost:
                    bands = CreateBands();
                    break;
                case Loudness:
                    bands = CreateBands();
                    break;
                case Headphones:
                    bands = CreateBands(0, 3, 6.8f, 3.2f, -2.6f, -1.8f, 0, 3, 6, 8);
                    break;
                case Acoustic:
                    bands = CreateBands();
                    break;
                case Classical:
                    bands = CreateBands(0, 0, 0, 0, 0, 0, 0, -4.8f, -4.8f, -6.4f);
                    break;
                case Dance:
                    bands = CreateBands(8, 6, 1.4f, 0, -1.6f, -3.6f, -4.8f, -4.8f, 0, 0);
                    break;
                case Club:
                    bands = CreateBands(0, 0, 2.6f, 3.6f, 3.6f, 3.6f, 2.6f, 0, 0);
                    break;
                case HipHop:
                    bands = CreateBands();
                    break;
                case Jazz:
                    bands = CreateBands();
                    break;
                case Pop:
                    bands = CreateBands(-2, -1.8f, 2.8f, 4.4f, 4.6f, 3.2f, 0, -1, -1.6f, -1.6f);
                    break;
                case Rock:
                    bands = CreateBands(8.0f, 4.8f, 2.8f, -3.6f, -5.2f, -2.6f, 2.6f, 5.6f, 6.8f, 6.8f);
                    break;
                case Party:
                    bands = CreateBands(4.4f, 4.4f, 4.4f, 0, 0, 0, 0, 0, 4.4f, 4.4f);
                    break;
                case Techno:
                    bands = CreateBands(5.4f, 4.8f, 3.2f, 0, -3.6f, -3.2f, 0, 4.8f, 5.8f, 6);
                    break;
                case Ska:
                    bands = CreateBands(-1.7f, -1.7f, -3.2f, -2.8f, -0.6f, 2.6f, 3.6f, 5.6f, 5.6f, 5.6f);
                    break;
                case Reggae:
                    bands = CreateBands(0, 0, 0, -0.6f, -3.6f, 0, 4.4f, 4.4f, 0, 0);
                    break;
                case Live:
                    bands = CreateBands(-3.8f, -3.2f, 0, 2.6f, 3.2f, 3.6f, 3.6f, 3.1f, 2.6f, 1.2f);
                    break;
                case RhythmAndBlues:
                    bands = CreateBands();
                    break;
                case Speech:
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
                var gain = gains != null && gains.Length > idx ? gains[idx] : 0;
                result.Add(frequency, gain);
                idx++;
            }

            return result;
        }
    }
}
