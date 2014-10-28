using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Authorization;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class MyMusicViewModel : AlbumsViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        private RelayCommand _setupAlbumsCommand;

        public MyMusicViewModel(IAuthorizationService authorizationService, IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, dialogService, navigationService)
        {
            _authorizationService = authorizationService;
            DataProvider.AudioAdded += OnAudioAdded;
            DataProvider.AudioRemoved += OnAudioRemoved;
        }

        public override string Title
        {
            get { return Resources.MyAudios; }
        }

        public override ICommand SetupAlbumsCommand
        {
            get
            {
                if (_setupAlbumsCommand == null)
                {
                    _setupAlbumsCommand = new RelayCommand(() => SetupAlbumsAsync(), () => !IsLoading);
                }

                return _setupAlbumsCommand;
            }
        }

        private Task SetupAlbumsAsync()
        {
            return Task.Factory.StartNew(SetupAlbums);
        }

        private void SetupAlbums()
        {
            SafeExecute(() =>
            {
                var user = _authorizationService.CurrentUser;

                if (user == null) { return; }

                var albumsCollection = DataProvider.GetUserAlbums(user);

                SetupAlbums(albumsCollection);

                if (AlbumsCollection != null &&
                    AlbumsCollection.Any() &&
                    CurrentAlbum == null)
                {
                    CurrentAlbum = AlbumsCollection.First();
                }
            },
            () => SetupAlbumsAsync());
        }

        private void OnAudioRemoved(object sender, AudioEventArgs args)
        {
            var audio = args.Audio;

            if (audio != null && Audios.Contains(audio))
            {
                Audios.Remove(audio);
            }
        }

        private void OnAudioAdded(object sender, AudioEventArgs args)
        {
            var audio = args.Audio;

            if (audio != null && !Audios.Contains(audio))
            {
                Audios.Insert(0, audio);
            }
        }
    }
}
