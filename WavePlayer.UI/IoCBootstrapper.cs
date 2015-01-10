using System;
using System.Collections.Generic;
using VK;
using WavePlayer.Authorization;
using WavePlayer.Configuration;
using WavePlayer.Ioc;
using WavePlayer.Localization;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Media;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Themes;
using WavePlayer.UI.ViewModels;
using WavePlayer.UI.ViewModels.Playlists;
using WavePlayer.UI.Windows;

namespace WavePlayer.UI
{
    internal static class IoCBootstrapper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It is OK")]
        public static IContainer GetContainer()
        {
            var container = Container.Instance;
            container.RegisterInstance(typeof(IContainer), container);

            var mainWindow = new HostWindow();
            container.RegisterInstance(typeof(IDialogService), mainWindow);
            container.RegisterInstance(typeof(HostWindow), mainWindow);
            
            container.Register<IConfigurationService, ConfigurationService>();
            container.Register<ILocalizationService, LocalizationService>();

#if DESIGN_DATA
            container.RegisterInstance(typeof(IPlayerEngine), DesignTime.DesignData.PlayerEngine);
            container.RegisterInstance(typeof(IAuthorizationService), DesignTime.DesignData.AuthorizationService);
            container.RegisterInstance(typeof(IVkDataProvider), DesignTime.DesignData.VkDataProvider);
            container.RegisterInstance(typeof(IPlayer), DesignTime.DesignData.Player);
#else
            container.Register<IPlayerEngine, PlayerEngine>();
            container.Register<IAuthorizationService, AuthorizationService>();
            container.Register<IVkDataProvider, VkDataProvider>();
            container.Register<IPlayer, Player>();
#endif
            container.Register<INavigationService, NavigationService>();
            container.Register<IThemeService, ThemeService>();
            container.Register<CommandsContainer>();
            container.Register<WavePlayerApp>();

            var viewModels = new[]
            {
                typeof(PlayerViewModel),
                typeof(WelcomeViewModel),
                typeof(LoginViewModel),
                typeof(LoadInfoViewModel),
                typeof(HostViewModel),
                typeof(MainViewModel),
                typeof(MyMusicViewModel),
                typeof(SettingsViewModel),
                typeof(PopularMusicViewModel),
                typeof(RecommendedMusicViewModel),
                typeof(SearchViewModel),
                typeof(NowPlayingViewModel),
                typeof(UsersViewModel),
                typeof(GroupsViewModel),
                typeof(LyricsViewModel),
                typeof(GroupMusicViewModel),
                typeof(UserMusicViewModel),
            };

            container.RegisterAll(viewModels);

            container.RegisterAll<PageViewModel>(
                typeof(MyMusicViewModel), 
                typeof(UsersViewModel), 
                typeof(GroupsViewModel), 
                typeof(RecommendedMusicViewModel), 
                typeof(PopularMusicViewModel), 
                typeof(SearchViewModel), 
                typeof(NowPlayingViewModel), 
                typeof(SettingsViewModel), 
                typeof(LyricsViewModel),
                typeof(GroupMusicViewModel),
                typeof(UserMusicViewModel));

            var localizables = new List<Type>(viewModels)
            {
                typeof(IVkDataProvider)
            };
            
            container.RegisterAll<ILocalizable>(localizables.ToArray());

            container.Register<VkClient>(() =>
            {
                var instance = Container.Instance;

                var configurationService = instance.GetInstance<IConfigurationService>();

                return new VkClient(configurationService.ApplicationId, configurationService.AccessRights);
            });

            return container;
        }
    }
}
