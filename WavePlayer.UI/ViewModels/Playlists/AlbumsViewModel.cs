using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Collections;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Threading;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public abstract class AlbumsViewModel : PlaylistViewModel
    {
        private Album _currentAlbum;

        private RelayCommand<Album> _setupAudiosCommand;
        private RelayCommand _loadAlbumsCommand;

        protected AlbumsViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, navigationService, dialogService)
        {
            DispatcherHelper.InvokeOnUI(() =>
            {
                Albums = new CustomObservableCollection<Album>();
            });
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string AlbumsCount
        {
            get
            {
                return Resources.AlbumsCount;
            }
        }

        public Album CurrentAlbum
        {
            get
            {
                return _currentAlbum;
            }

            set
            {
                SetField(ref _currentAlbum, value);
            }
        }

        public CustomObservableCollection<Album> Albums
        {
            get;
            private set;
        }

        public override ICommand SetupAudiosCommand
        {
            get
            {
                if (_setupAudiosCommand == null)
                {
                    _setupAudiosCommand = new RelayCommand<Album>((album) => SetupAudiosAsync(album), (album) => !IsLoading);
                }

                return _setupAudiosCommand;
            }
        }

        public ICommand LoadAlbumsCommand
        {
            get
            {
                if (_loadAlbumsCommand == null)
                {
                    _loadAlbumsCommand = new RelayCommand(() => LoadAlbumsAsync(), () => CanLoadCollection(AlbumsCollection));
                }

                return _loadAlbumsCommand;
            }
        }

        public abstract ICommand SetupAlbumsCommand
        {
            get;
        }

        protected ICollection<Album> AlbumsCollection { get; private set; }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();
            RaisePropertyChanged("AlbumsCount");
        }

        protected void SetupAlbums(ICollection<Album> albumsCollection, Album album)
        {
            AlbumsCollection = albumsCollection;

            Albums.Reset(AlbumsCollection);

            var currentAlbum = album != null ? albumsCollection.FirstOrDefault(a => a.Id == album.Id && a.OwnerId == album.OwnerId && a.OwnerIsGroup == album.OwnerIsGroup) : albumsCollection.DefaultAlbum();

            CurrentAlbum = currentAlbum;

            RaisePropertyChanged("AlbumsCount");
        }

        protected void ResetAlbums()
        {
            ResetAudios();

            if (AlbumsCollection == null && Albums.Count == 0)
            {
                return;
            }

            AlbumsCollection = null;

            Albums.Reset(Enumerable.Empty<Album>());

            CurrentAlbum = null;
        }
        
        protected void SetupAudios(Album album)
        {
            ResetAudios();

            CurrentAlbum = album;

            if (album == null)
            {
                return;
            }

            try
            {
                var audiosCollection = DataProvider.GetAlbumAudios(album);

                SetupAudios(audiosCollection);
            }
            catch
            {
                ResetAudios();
                throw;
            }
        }

        private Task SetupAudiosAsync(Album album)
        {
            return Task.Factory.StartNew(() => SafeExecute(() => SetupAudios(album), () => SetupAudiosAsync(album)));
        }

        private Task LoadAlbumsAsync()
        {
            return Task.Factory.StartNew(LoadAlbums);
        }

        private void LoadAlbums()
        {
            SafeExecute(() =>
            {
                if (AlbumsCollection == null)
                {
                    return;
                }

                var count = AlbumsCollection.Count;

                DataProvider.LoadCollection(AlbumsCollection);

                Albums.AddRange(AlbumsCollection.Skip(count));
            },
            () => LoadAlbumsAsync());
        }
    }
}
