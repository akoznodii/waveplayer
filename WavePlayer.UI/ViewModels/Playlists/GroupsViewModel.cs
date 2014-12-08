using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Authorization;
using WavePlayer.Groups;
using WavePlayer.Providers;
using WavePlayer.UI.Collections;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Threading;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class GroupsViewModel : PageViewModel, IItemsViewModel<Group>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IVkDataProvider _dataProvider;
        private readonly GroupMusicViewModel _groupMusicViewModel;
        private RelayCommand _setupGroupsCommand;
        private RelayCommand<Group> _selectUserCommand;
        private ICollection<Group> _groupsCollection;
        
        public GroupsViewModel(GroupMusicViewModel groupMusicViewModel, IAuthorizationService authorizationService, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(navigationService, dialogService)
        {
            _groupMusicViewModel = groupMusicViewModel;
            _authorizationService = authorizationService;
            _dataProvider = dataProvider;

            DispatcherHelper.InvokeOnUI(() =>
            {
                Items = new CustomObservableCollection<Group>();
            });

            LoadItemsCommand = new RelayCommand(() => Async(() => LoadCollection(_dataProvider, Items, _groupsCollection)), () => CanLoadCollection(_groupsCollection));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ItemsCount
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

        public CustomObservableCollection<Group> Items
        {
            get;
            private set;
        }

        public ICommand SetupItemsCommand
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

        public ICommand SelectItemCommand
        {
            get
            {
                if (_selectUserCommand == null)
                {
                    _selectUserCommand = new RelayCommand<Group>((group) => SelectGroupAsync(group));
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

            ResetGroups();

            SetupGroups();
        }

        private Task SetupGroupsAsync()
        {
            return Async(() => SafeExecute(SetupGroups, () => SetupGroupsAsync()));
        }

        private void SetupGroups()
        {
            ResetGroups();

            var user = _authorizationService.CurrentUser;

            _groupsCollection = _dataProvider.GetUserGroups(user);

            Items.Reset(_groupsCollection);
        }

        private void ResetGroups()
        {
            if (_groupsCollection == null && Items.Count == 0)
            {
                return;
            }

            _groupsCollection = null;

            Items.Reset(Enumerable.Empty<Group>());
        }

        private Task SelectGroupAsync(Group group)
        {
            return Async(() => SafeExecute(() => SelectGroup(group), () => SelectGroupAsync(group)));
        }

        private void SelectGroup(Group group)
        {
            if (group == null)
            {
                return;
            }

            while (_groupMusicViewModel.IsLoading)
            {
                Thread.Sleep(100);
            }

            _groupMusicViewModel.LoadGroupAlbums(group);
        }
    }
}
