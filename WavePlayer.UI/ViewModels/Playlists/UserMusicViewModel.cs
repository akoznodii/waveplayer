using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.Users;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class UserMusicViewModel : AlbumsViewModelBase
    {
        private string _title;
        private User _currentUser;

        public UserMusicViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, dialogService, navigationService)
        {
        }

        public override ICommand SetupAlbumsCommand
        {
            get { return null; }
        }

        public new string Title
        {
            get
            {
                return _title;
            }

            private set
            {
                SetField(ref _title, value);
            }
        }

        public void LoadUserAlbums(User user)
        {
            LoadUserAlbums(user, null);
        }

        protected override void Reload()
        {
            base.Reload();

            var album = CurrentAlbum;
            var currentUser = _currentUser;

            LoadUserAlbums(currentUser, album);
        }

        private void LoadUserAlbums(User user, Album album)
        {
            ResetAlbums();

            _currentUser = user;

            if (_currentUser == null)
            {
                Title = string.Empty;
                return;
            }

            Title = string.Format(Resources.Culture, "{0} {1}", _currentUser.FirstName, _currentUser.LastName);

            var albumsCollection = DataProvider.GetUserAlbums(_currentUser);

            SetupAlbums(albumsCollection, album);
        }
    }
}
