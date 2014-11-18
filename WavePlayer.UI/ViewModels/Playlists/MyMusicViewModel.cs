using System.Linq;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Authorization;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class MyMusicViewModel : AlbumsViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        
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
            get { return null; }
        }

        protected override void Reload()
        {
            base.Reload();

            var album = CurrentAlbum;

            ResetAlbums();

            var user = _authorizationService.CurrentUser;

            var albumsCollection = DataProvider.GetUserAlbums(user);

            SetupAlbums(albumsCollection, album);
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
