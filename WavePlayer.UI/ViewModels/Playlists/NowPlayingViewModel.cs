using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class NowPlayingViewModel : PlaylistViewModel
    {
        private RelayCommand _setupAudiosCommand;

        public NowPlayingViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, navigationService, dialogService)
        {
        }

        public override string Title
        {
            get { return Resources.NowPlaying; }
        }

        public override ICommand SetupAudiosCommand
        {
            get
            {
                if (_setupAudiosCommand == null)
                {
                    _setupAudiosCommand = new RelayCommand(() => SetupAudiosAsync(), () =>
                    {
                        var tracklist = Player.Tracklist as Tracklist<Audio>;

                        return tracklist != null && CanLoadCollection(tracklist.Source);
                    });
                }

                return _setupAudiosCommand;
            }
        }

        private Task SetupAudiosAsync()
        {
            return Task.Factory.StartNew(SetupAudios);
        }

        private void SetupAudios()
        {
            SafeExecute(() =>
            {
                var tracklist = Player.Tracklist as Tracklist<Audio>;

                if (tracklist == null)
                {
                    return;
                }

                SetupAudios(tracklist.Source);
            },
            () => SetupAudiosAsync());
        }
    }
}
