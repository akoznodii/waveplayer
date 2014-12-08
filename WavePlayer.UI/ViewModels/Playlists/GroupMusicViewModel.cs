using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Groups;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class GroupMusicViewModel : AlbumsViewModelBase, INavigatable
    {
        private string _title;
        private Group _currentGroup;

        public GroupMusicViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
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

        public void LoadGroupAlbums(Group group)
        {
            LoadGroupAlbums(group, null);

            NavigationService.Navigate(this, group);
        }

        public void OnNavigated(object parameter)
        {
            var group = parameter as Group;

            if (group == null || _currentGroup == group) { return; }

            SetupAlbumsAsync(group);
        }

        protected override void Reload()
        {
            base.Reload();

            var album = CurrentAlbum;
            var currentGroup = _currentGroup;

            LoadGroupAlbums(currentGroup, album);
        }

        private void SetupAlbumsAsync(Group group)
        {
            Async(() => SafeExecute(() => LoadGroupAlbums(group, null), () => SetupAlbumsAsync(group)));
        }

        private void LoadGroupAlbums(Group group, Album album)
        {
            ResetAlbums();

            _currentGroup = group;

            if (_currentGroup == null)
            {
                Title = string.Empty;
                return;
            }

            Title = string.Format(Resources.Culture, "{0}", _currentGroup.Name);

            var albumsCollection = DataProvider.GetGroupAlbums(_currentGroup);

            SetupAlbums(albumsCollection, album);
        }
    }
}
