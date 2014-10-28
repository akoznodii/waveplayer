using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels
{
    public class LyricsViewModel : PageViewModel, INavigatable
    {
        private readonly IVkDataProvider _vkDataProvider;
        private RelayCommand<Audio> _loadLyrics;
        private Lyrics _currentLyrics;

        public LyricsViewModel(IVkDataProvider vkDataProvider, INavigationService navigationService, IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _vkDataProvider = vkDataProvider;
        }

        public override string Title
        {
            get
            {
                return Resources.SongLyrics;
            }
        }

        public Lyrics CurrentLyrics
        {
            get
            {
                return _currentLyrics;
            }

            set
            {
                SetField(ref _currentLyrics, value);
            }
        }

        public ICommand SetupLyricsCommand
        {
            get
            {
                if (_loadLyrics == null)
                {
                    _loadLyrics = new RelayCommand<Audio>((a) => SetupLyricsAsync(a), CanExecute);
                }

                return _loadLyrics;
            }
        }

        public void OnNavigated(object parameter)
        {
            var audio = parameter as Audio;

            if (CanExecute(audio))
            {
                SetupLyricsAsync(audio);
            }
        }

        private bool CanExecute(Audio audio)
        {
            return audio != null && audio.LyricsId != 0;
        }

        private Task SetupLyricsAsync(Audio audio)
        {
            return Task.Factory.StartNew(() => SetupLyrics(audio));
        }

        private void SetupLyrics(Audio audio)
        {
            SafeExecute(() =>
            {
                NavigationService.Navigate(this);

                if (CurrentLyrics != null &&
                    audio != null &&
                    CurrentLyrics.Id != audio.LyricsId)
                {
                    CurrentLyrics = null;
                }

                CurrentLyrics = _vkDataProvider.GetLyrics(audio);
            },
           () => SetupLyricsAsync(audio));
        }
    }
}
