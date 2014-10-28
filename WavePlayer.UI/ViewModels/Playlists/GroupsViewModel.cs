using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Authorization;
using WavePlayer.Groups;
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
    public class GroupsViewModel : AlbumsViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        private Group _currentGroup;
        private RelayCommand _setupGroupsCommand;
        private RelayCommand _loadGroupsCommand;
        private RelayCommand<Group> _setupAlbumsCommand;
        private ICollection<Group> _groupsCollection;

        public GroupsViewModel(IAuthorizationService authorizationService, IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, dialogService, navigationService)
        {
            _authorizationService = authorizationService;

            DispatcherHelper.InvokeOnUI(() =>
            {
                Groups = new CustomObservableCollection<Group>();
            });
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string GroupsCount
        {
            get
            {
                return Resources.GroupsCount;
            }
        }

        public override string Title
        {
            get { return Resources.MyGroups; }
        }

        public Group CurrentGroup
        {
            get
            {
                return _currentGroup;
            }

            set
            {
                SetField(ref _currentGroup, value);
            }
        }

        public CustomObservableCollection<Group> Groups
        {
            get;
            private set;
        }

        public ICommand SetupGroupsCommand
        {
            get
            {
                if (_setupGroupsCommand == null)
                {
                    _setupGroupsCommand = new RelayCommand(() => SetupGroupsAsync(), () => !IsLoading);
                }

                return _setupGroupsCommand;
            }
        }

        public ICommand LoadGroupsCommand
        {
            get
            {
                if (_loadGroupsCommand == null)
                {
                    _loadGroupsCommand = new RelayCommand(() => LoadGroupsAsync(), () => CanLoadCollection(_groupsCollection));
                }

                return _loadGroupsCommand;
            }
        }

        public override ICommand SetupAlbumsCommand
        {
            get
            {
                if (_setupAlbumsCommand == null)
                {
                    _setupAlbumsCommand = new RelayCommand<Group>((group) => SetupAlbumsAsync(group), (group) => !IsLoading);
                }

                return _setupAlbumsCommand;
            }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();
            RaisePropertyChanged("GroupsCount");
        }

        private Task SetupAlbumsAsync(Group group)
        {
            return Task.Factory.StartNew(() => SetupAlbums(group));
        }

        private void SetupAlbums(Group group)
        {
            SafeExecute(() =>
            {
                if (group == null)
                {
                    return;
                }

                var albumsCollection = DataProvider.GetGroupAlbums(group);

                SetupAlbums(albumsCollection);

                CurrentGroup = group;

                var album = AlbumsCollection != null ? AlbumsCollection.FirstOrDefault() : null;

                if (album != null)
                {
                    SetupAudiosCommand.Execute(album);
                }
            },
            () => SetupAlbumsAsync(group));
        }

        private Task SetupGroupsAsync()
        {
            return Task.Factory.StartNew(SetupGroups);
        }

        private void SetupGroups()
        {
            SafeExecute(() =>
            {
                var user = _authorizationService.CurrentUser;

                if (user == null)
                {
                    return;
                }

                _groupsCollection = DataProvider.GetUserGroups(user);

                Groups.Reset(_groupsCollection);

                RaisePropertyChanged("GroupsCount");

                if (_groupsCollection != null &&
                    _groupsCollection.Any() &&
                    CurrentGroup == null)
                {
                    CurrentGroup = _groupsCollection.First();
                }
            },
            () => SetupGroupsAsync());
        }

        private Task LoadGroupsAsync()
        {
            return Task.Factory.StartNew(LoadGroups);
        }

        private void LoadGroups()
        {
            SafeExecute(() =>
            {
                if (_groupsCollection == null)
                {
                    return;
                }

                var count = _groupsCollection.Count;

                DataProvider.LoadCollection(_groupsCollection);

                Groups.AddRange(_groupsCollection.Skip(count));

                RaisePropertyChanged("GroupsCount");
            },
            () => LoadGroupsAsync());
        }
    }
}
