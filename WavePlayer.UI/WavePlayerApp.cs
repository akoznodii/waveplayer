using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using VK;
using WavePlayer.Authorization;
using WavePlayer.Configuration;
using WavePlayer.Ioc;
using WavePlayer.Localization;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Themes;
using WavePlayer.UI.Threading;
using WavePlayer.UI.ViewModels;
using WavePlayer.UI.ViewModels.Playlists;

namespace WavePlayer.UI
{
    public class WavePlayerApp
    {
        public WavePlayerApp(IContainer container)
        {
            Container = container;
        }

        public IContainer Container { get; private set; }

        public void Initialize()
        {
            DispatcherHelper.Initialize();

            SetupNavigation();

            SetupLocalization();

            SetupTheme();

            SetupPages();
        }

        public void Start()
        {
            var configurationService = Container.GetInstance<IConfigurationService>();
            var accessToken = configurationService.AccessToken;

            if (accessToken == null || accessToken.Expired())
            {
                var navigationService = Container.GetInstance<INavigationService>();
                navigationService.Navigate<LoginViewModel>();
            }
            else
            {
                var authorizationService = Container.GetInstance<IAuthorizationService>();
                authorizationService.SetAccessToken(accessToken);
                var loginViewModel = Container.GetInstance<LoginViewModel>();
                loginViewModel.LoadUserInfoAsync();
            }
        }

        public void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var application = Application.Current;

            var dialogService = Container.GetInstance<IDialogService>();

            var affirmativeAction = new Action(() =>
            {
                Process.Start(Assembly.GetExecutingAssembly().Location);
                application.Shutdown();
            });

            var negativeAction = new Action(() =>
            {
                application.Shutdown();
            });

            var dialogMessage = new DialogMessage()
            {
                Title = Resources.OopsSomethingWentWrong,
                Message = e.Exception.Message,
                AffirmativeAction = affirmativeAction,
                AffirmativeActionText = Resources.Restart,
                NegativeAction = negativeAction,
                NegativeActionText = Resources.Close
            };

            dialogService.NotifyMessage(dialogMessage);

            e.Handled = true;
        }

        private void SetupLocalization()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var localizationService = Container.GetInstance<ILocalizationService>();
            var configurtionService = Container.GetInstance<IConfigurationService>();

            localizationService.CurrentCultureChanged += OnCurrentCultureChanged;

            localizationService.RegisterResource(typeof(VkLocalization));
            localizationService.RegisterResource(typeof(WavePlayerLocalization));
            localizationService.RegisterResource(typeof(Resources));

            localizationService.SetCurrentCulture(configurtionService.CultureId, true);
        }

        private void SetupTheme()
        {
            var themeService = Container.GetInstance<IThemeService>();
            var configurtionService = Container.GetInstance<IConfigurationService>();

            themeService.ChangeTheme(configurtionService.Theme, configurtionService.AccentColor, true);
        }

        private void SetupPages()
        {
            var mainViewModel = Container.GetInstance<MainViewModel>();
            var instances = Container.GetAllInstances<PageViewModel>()
                .Where(viewModel => !(viewModel is LyricsViewModel) && !(viewModel is GroupMusicViewModel) && !(viewModel is UserMusicViewModel));

            mainViewModel.Views = new List<PageViewModel>(instances);
        }

        private void SetupNavigation()
        {
            var navigationService = Container.GetInstance<INavigationService>();
            var hostViewModel = Container.GetInstance<HostViewModel>();
            var mainViewModel = Container.GetInstance<MainViewModel>();
            var welcomeViewModel = Container.GetInstance<WelcomeViewModel>();

            navigationService.SetupNavigationRule(hostViewModel, welcomeViewModel);
            navigationService.SetupNavigationRule(hostViewModel, mainViewModel);
            navigationService.SetupNavigationRule(welcomeViewModel, Container.GetInstance<LoginViewModel>());
            navigationService.SetupNavigationRule(welcomeViewModel, Container.GetInstance<LoadInfoViewModel>());

            var pageViewModels = Container.GetAllInstances<PageViewModel>();

            foreach (var pageViewModel in pageViewModels)
            {
                navigationService.SetupNavigationRule(mainViewModel, pageViewModel, true);
            }
        }

        private void OnCurrentCultureChanged(object sender, EventArgs args)
        {
            var localizationService = sender as ILocalizationService;

            if (localizationService == null) { return; }

            var instances = Container.GetAllInstances<ILocalizable>();

            foreach (var instance in instances)
            {
                instance.UpdateLocalization();
            }
        }
    }
}
