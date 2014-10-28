using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Authorization;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class RecommendedMusicViewModel : PlaylistViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        private RelayCommand _setupAudiosCommand;

        public RecommendedMusicViewModel(IAuthorizationService authorizationService, IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, navigationService, dialogService)
        {
            _authorizationService = authorizationService;
        }

        public override string Title
        {
            get { return Resources.Recommendations; }
        }

        public override ICommand SetupAudiosCommand
        {
            get
            {
                if (_setupAudiosCommand == null)
                {
                    _setupAudiosCommand = new RelayCommand(() => SetupAudiosAsync(), () => !IsLoading);
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
                var user = _authorizationService.CurrentUser;

                if (user == null)
                {
                    return;
                }

                var audiosCollection = DataProvider.GetRecommendedAudios(user, false);

                SetupAudios(audiosCollection);
            },
            () => SetupAudiosAsync());
        }
    }
}
