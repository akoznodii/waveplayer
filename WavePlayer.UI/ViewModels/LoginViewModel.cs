using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.Authorization;
using WavePlayer.UI.ViewModels.Playlists;

namespace WavePlayer.UI.ViewModels
{
    public sealed class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IVkDataProvider _vkDataProvider;
        private RelayCommand _singupCommand;

        public LoginViewModel(INavigationService navigationService, IDialogService dialogService, IAuthorizationService authorizationService, IVkDataProvider vkDataProvider)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authorizationService = authorizationService;
            _vkDataProvider = vkDataProvider;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string SignInString
        {
            get { return Resources.SignInVk; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string SignupString
        {
            get { return Resources.SignUpVk; }
        }

        public Uri LoginUri
        {
            get
            {
                return _authorizationService.LoginUri;
            }
        }

        public bool CanRetrieveToken(Uri uri)
        {
            return _authorizationService.CanRetrieveToken(uri);
        }

        public void RetrieveToken(Uri uri)
        {
            _authorizationService.RetrieveAccessToken(uri);

            LoadUserInfoAsync();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should be safe method")]
        public ICommand SignupCommand
        {
            get
            {
                if (_singupCommand == null)
                {
                    _singupCommand = new RelayCommand(() =>
                    {
                        try
                        {
                            Process.Start(_authorizationService.SignupUri.OriginalString);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Failed to open VK registration page{0}Error: {1}", Environment.NewLine, e);
                        }
                    });
                }

                return _singupCommand;
            }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();

            RaisePropertyChanged("SignInString");
            RaisePropertyChanged("SignupString");
        }

        public Task LoadUserInfoAsync()
        {
            return Task.Factory.StartNew(LoadUserInfo);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should be safe method")]
        private void LoadUserInfo()
        {
            _navigationService.Navigate<LoadInfoViewModel>();

            try
            {
                _authorizationService.TrackUser();

                _authorizationService.LoadUserInfo();
                
                var user = _authorizationService.CurrentUser;

                var albums = _vkDataProvider.GetUserAlbums(user);
                
                var album = albums.Single(a => a.Id == Album.AllMusicAlbumId);

                _vkDataProvider.GetAlbumAudios(album);

                _navigationService.Navigate<MyMusicViewModel>();
            }
            catch (Exception exception)
            {
                _dialogService.NotifyError(exception, () => LoadUserInfoAsync());
            }
        }
    }
}
