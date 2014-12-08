using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Authorization;
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
    public class UsersViewModel : PageViewModel, IItemsViewModel<User>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IVkDataProvider _dataProvider;
        private readonly UserMusicViewModel _userMusicViewModel;
        private RelayCommand _setupUsersCommand;
        private RelayCommand<User> _selectUserCommand;
        private ICollection<User> _usersCollection;

        public UsersViewModel(UserMusicViewModel userMusicViewModel, IAuthorizationService authorizationService, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(navigationService, dialogService)
        {
            _userMusicViewModel = userMusicViewModel;
            _authorizationService = authorizationService;
            _dataProvider = dataProvider;

            DispatcherHelper.InvokeOnUI(() =>
            {
                Items = new CustomObservableCollection<User>();
            });

            LoadItemsCommand = new RelayCommand(() => Async(() => LoadCollection(_dataProvider, Items, _usersCollection)), () => CanLoadCollection(_usersCollection));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ItemsCount
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

        public CustomObservableCollection<User> Items
        {
            get;
            private set;
        }

        public ICommand SetupItemsCommand
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

        public ICommand SelectItemCommand
        {
            get
            {
                if (_selectUserCommand == null)
                {
                    _selectUserCommand = new RelayCommand<User>((user) => SelectUserAsync(user));
                }

                return _selectUserCommand;
            }
        }

        public ICommand LoadItemsCommand
        {
            get; private set;
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();
            RaisePropertyChanged("ItemsCount");
        }

        protected override void Reload()
        {
            base.Reload();

            ResetUsers();

            SetupUsers();
        }

        private Task SetupUsersAsync()
        {
            return Async(() => SafeExecute(SetupUsers, () => SetupUsersAsync()));
        }

        private void SetupUsers()
        {
            ResetUsers();

            var user = _authorizationService.CurrentUser;

            _usersCollection = _dataProvider.GetUserFriends(user);

            Items.Reset(_usersCollection);
        }

        private void ResetUsers()
        {
            if (_usersCollection == null && Items.Count == 0)
            {
                return;
            }

            _usersCollection = null;

            Items.Reset(Enumerable.Empty<User>());
        }

        private Task SelectUserAsync(User user)
        {
            return Async(() => SafeExecute(() => SelectUser(user), () => SelectUserAsync(user)));
        }

        private void SelectUser(User user)
        {
            if (user == null)
            {
                return;
            }

            while (_userMusicViewModel.IsLoading)
            {
                Thread.Sleep(100);
            }

            _userMusicViewModel.LoadUserAlbums(user);
        }
    }
}
