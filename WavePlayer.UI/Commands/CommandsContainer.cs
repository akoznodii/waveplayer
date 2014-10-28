using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Authorization;
using WavePlayer.Ioc;
using WavePlayer.Providers;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.ViewModels;
using WavePlayer.UI.ViewModels.Playlists;

namespace WavePlayer.UI.Commands
{
    public class CommandsContainer
    {
        private readonly IVkDataProvider _vkDataProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        public CommandsContainer(IVkDataProvider vkDataProvider, IAuthorizationService authorizationService, IDialogService dialogService, INavigationService navigationService)
        {
            _vkDataProvider = vkDataProvider;
            _authorizationService = authorizationService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            
            Initialize();
        }

        public static CommandsContainer Instance
        {
            get
            {
                return Container.Instance.GetInstance<CommandsContainer>();
            }
        }

        public ICommand AddAudioCommand { get; private set; }

        public ICommand RemoveAudioCommand { get; private set; }

        public ICommand SearchByArtistAudioCommand { get; private set; }

        public ICommand SearchByTitleAudioCommand { get; private set; }

        public ICommand ShowAudioLyricsCommand { get; private set; }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string AddAudioCommandName { get { return Resources.AddAudioCommandName; } }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string RemoveAudioCommandName { get { return Resources.RemoveAudioCommandName; } }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string SearchByArtistAudioCommandName { get { return Resources.SearchByArtistAudioCommandName; } }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string SearchByTitleAudioCommandName { get { return Resources.SearchByTitleAudioCommandName; } }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ShowAudioLyricsCommandName { get { return Resources.ShowAudioLyricsCommandName; } }

        private void AddAudio(Audio audio)
        {
            SafeExecute(() => _vkDataProvider.Add(audio));
        }

        private void RemoveAudio(Audio audio)
        {
            SafeExecute(() => _vkDataProvider.Remove(audio));
        }

        private bool CanAddAudio(Audio audio)
        {
            var currentUser = _authorizationService.CurrentUser;

            return audio != null &&
                   !audio.OwnerIsGroup &&
                   currentUser != null &&
                   audio.OwnerId != currentUser.Id;
        }

        private bool CanRemoveAudio(Audio audio)
        {
            var currentUser = _authorizationService.CurrentUser;

            return audio != null &&
                   !audio.OwnerIsGroup &&
                   currentUser != null &&
                   audio.OwnerId == currentUser.Id;
        }
        
        private void SearchAudio(string query)
        {
            SafeExecute(() => _navigationService.Navigate<SearchViewModel>(query));
        }

        private void Initialize()
        {
            AddAudioCommand = new RelayCommand<Audio>(AddAudio, CanAddAudio);
            RemoveAudioCommand = new RelayCommand<Audio>(RemoveAudio, CanRemoveAudio);
            SearchByArtistAudioCommand = new RelayCommand<Audio>(audio => SearchAudio(audio.Artist), audio => audio != null && !string.IsNullOrEmpty(audio.Artist));
            SearchByTitleAudioCommand = new RelayCommand<Audio>(audio => SearchAudio(audio.Title), audio => audio != null && !string.IsNullOrEmpty(audio.Title));
            ShowAudioLyricsCommand = Container.Instance.GetInstance<LyricsViewModel>().SetupLyricsCommand;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Show error message to the end-user")]
        private void SafeExecute(Action action, Action retryAction = null)
        {
            Exception exception = null;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception != null)
            {
                if (retryAction == null)
                {
                    _dialogService.NotifyError(exception);
                }
                else
                {
                    _dialogService.NotifyError(exception, retryAction);
                }
            }
        }
    }
}
