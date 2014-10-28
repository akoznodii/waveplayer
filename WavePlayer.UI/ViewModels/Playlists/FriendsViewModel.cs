using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Authorization;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Collections;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Threading;
using WavePlayer.Users;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class FriendsViewModel : AlbumsViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        private User _currentUser;
        private RelayCommand _setupUsersCommand;
        private RelayCommand _loadUsersCommand;
        private RelayCommand<User> _setupAlbumsCommand;
        private ICollection<User> _friendsCollection;

        public FriendsViewModel(IAuthorizationService authorizationService, IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, dialogService, navigationService)
        {
            _authorizationService = authorizationService;

            DispatcherHelper.InvokeOnUI(() =>
            {
                Users = new CustomObservableCollection<User>();
            });
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string UsersCount
        {
            get
            {
                return Resources.UsersCount;
            }
        }

        public override string Title
        {
            get { return Resources.MyFriends; }
        }

        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }

            set
            {
                SetField(ref _currentUser, value);
            }
        }

        public CustomObservableCollection<User> Users
        {
            get;
            private set;
        }

        public ICommand SetupUsersCommand
        {
            get
            {
                if (_setupUsersCommand == null)
                {
                    _setupUsersCommand = new RelayCommand(() => SetupUsersAsync(), () => !IsLoading);
                }

                return _setupUsersCommand;
            }
        }

        public ICommand LoadUsersCommand
        {
            get
            {
                if (_loadUsersCommand == null)
                {
                    _loadUsersCommand = new RelayCommand(() => LoadUsersAsync(), () => CanLoadCollection(_friendsCollection));
                }

                return _loadUsersCommand;
            }
        }

        public override ICommand SetupAlbumsCommand
        {
            get
            {
                if (_setupAlbumsCommand == null)
                {
                    _setupAlbumsCommand = new RelayCommand<User>((user) => SetupAlbumsAsync(user), (user) => !IsLoading);
                }

                return _setupAlbumsCommand;
            }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();
            RaisePropertyChanged("UsersCount");
        }

        private Task SetupAlbumsAsync(User user)
        {
            return Task.Factory.StartNew(() => SetupAlbums(user));
        }

        private void SetupAlbums(User user)
        {
            SafeExecute(() =>
            {
                if (user == null)
                {
                    return;
                }

                var albumsCollection = DataProvider.GetUserAlbums(user);

                SetupAlbums(albumsCollection);

                CurrentUser = user;

                var album = AlbumsCollection != null ? AlbumsCollection.FirstOrDefault() : null;

                if (album != null)
                {
                    SetupAudiosCommand.Execute(album);
                }
            },
            () => SetupAlbumsAsync(user));
        }

        private Task SetupUsersAsync()
        {
            return Task.Factory.StartNew(SetupUsers);
        }

        private void SetupUsers()
        {
            SafeExecute(() =>
            {
                var user = _authorizationService.CurrentUser;

                if (user == null)
                {
                    return;
                }

                _friendsCollection = DataProvider.GetUserFriends(user);

                Users.Reset(_friendsCollection);

                RaisePropertyChanged("UsersCount");

                if (_friendsCollection != null &&
                    _friendsCollection.Any() &&
                    CurrentUser == null)
                {
                    CurrentUser = _friendsCollection.First();
                }
            },
            () => SetupUsersAsync());
        }

        private Task LoadUsersAsync()
        {
            return Task.Factory.StartNew(LoadUsers);
        }

        private void LoadUsers()
        {
            SafeExecute(() =>
            {
                if (_friendsCollection == null)
                {
                    return;
                }

                var count = _friendsCollection.Count;

                DataProvider.LoadCollection(_friendsCollection);

                Users.AddRange(_friendsCollection.Skip(count));

                RaisePropertyChanged("UsersCount");
            },
            () => LoadUsersAsync());
        }
    }
}
