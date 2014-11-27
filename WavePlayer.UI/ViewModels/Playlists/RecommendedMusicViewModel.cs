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
    public class RecommendedMusicViewModel : MusicViewModelBase
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
            return Async(() => SafeExecute(SetupAudios, () => SetupAudiosAsync()));
        }

        private void SetupAudios()
        {
            var user = _authorizationService.CurrentUser;

            if (user == null)
            {
                return;
            }

            var audiosCollection = DataProvider.GetRecommendedAudios(user, false);

            SetupAudios(audiosCollection);
        }
    }
}
