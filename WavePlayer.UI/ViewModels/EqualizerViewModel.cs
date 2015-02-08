using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WavePlayer.Media;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels
{
    public class EqualizerViewModel : PageViewModel
    {
        private readonly IEqualizer _equalizer;
        private readonly Collection<EqualizerBandViewModel> _bands; 

        private RelayCommand _resetCommand;

        public EqualizerViewModel(INavigationService navigationService,
                                  IDialogService dialogService,
                                  IPlayerEngine playerEngine)
            : base(navigationService, dialogService)
        {
            _equalizer = playerEngine.Equalizer;

            _bands = new Collection<EqualizerBandViewModel>();

            if (_equalizer == null)
            {
                return;
            }

            foreach (var frequency in _equalizer.FrequencyRange)
            {
                var band = new EqualizerBandViewModel(frequency, _equalizer);
                _bands.Add(band);
            }

            _equalizer.PresetChanged += OnPresetChanged;
        }

        public override string Title
        {
            get
            {
                return Resources.Equalizer;
            }
        }

        public bool SupportsEqualizer { get { return _equalizer != null; } }

        public string State { get { return SupportsEqualizer && _equalizer.IsEnabled ? Resources.On : Resources.Off; } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is OK")]
        public IEnumerable<EqualizerPreset> Presets { get { return EqualizerPreset.DefaultPresets.Values; } }
        
        public IEnumerable<EqualizerBandViewModel> Bands { get { return _bands; } }

        public EqualizerPreset CurrentPreset
        {
            get { return SupportsEqualizer ? _equalizer.CurrentPreset : null; }
            
            set
            {
                if (SupportsEqualizer)
                {
                    _equalizer.CurrentPreset = value;
                }
            }
        }

        public bool IsEnabled
        {
            get { return SupportsEqualizer && _equalizer.IsEnabled; }

            set
            {
                if (SupportsEqualizer)
                {
                    _equalizer.IsEnabled = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(ResetEqualizer, CanResetEqualizer);    
                }

                return _resetCommand;
            }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();

            if (_equalizer != null)
            {
                _equalizer.UpdateLocalization();
            }
        }

        private bool CanResetEqualizer()
        {
            return SupportsEqualizer && _equalizer.CurrentPreset.Name == EqualizerPreset.Manual;
        }

        private void ResetEqualizer()
        {
            _equalizer.Reset();
        }

        private void OnPresetChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("CurrentPreset");

            foreach (var band in Bands)
            {
                band.Refresh();
            }
        }
    }
}
