using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using VK.Audios;
using WavePlayer.Ioc;
using WavePlayer.UI.ViewModels;
using WavePlayer.UI.ViewModels.Playlists;

namespace WavePlayer.UI.DesignTime
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "It is called on application start")]
    internal class DesignTimeViewModelsLocator
    {
        private static readonly IContainer Container = App.Container;
        private static WavePlayerApp _wavePlayerApp;

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public MyMusicViewModel MyMusicViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<MyMusicViewModel>();

                viewModel.SetupAlbumsCommand.Execute(null);

                System.Threading.Thread.Sleep(100);

                var album = viewModel.Albums.First();

                viewModel.SetupAudiosCommand.Execute(album);

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public PopularMusicViewModel PopularMusicViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<PopularMusicViewModel>();

                viewModel.ReloadCommand.Execute(null);

                System.Threading.Thread.Sleep(100);

                var genre = viewModel.Genres.First();

                viewModel.SetupAudiosCommand.Execute(genre);

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public RecommendedMusicViewModel RecommendedMusicViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<RecommendedMusicViewModel>();

                viewModel.SetupAudiosCommand.Execute(null);

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public SearchViewModel SearchViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<SearchViewModel>();

                viewModel.SetupAudiosCommand.Execute(string.Empty);

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public NowPlayingViewModel NowPlayingViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<NowPlayingViewModel>();

                viewModel.SetupAudiosCommand.Execute(null);

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public UsersViewModel UsersViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<UsersViewModel>();

                viewModel.ReloadCommand.Execute(null);
                
                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public GroupsViewModel GroupsViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<GroupsViewModel>();

                viewModel.ReloadCommand.Execute(null);

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<SettingsViewModel>();

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public MainViewModel MainViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<MainViewModel>();

                var page = viewModel.Views.OfType<UsersViewModel>().First();

                viewModel.SelectView = page;

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public WelcomeViewModel WelcomeViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<WelcomeViewModel>();
                var loginViewModel = Container.GetInstance<LoginViewModel>();
                
                viewModel.CurrentView = loginViewModel;

                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public PlayerViewModel PlayerViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<PlayerViewModel>();
                
                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "WPF requires property")]
        public LyricsViewModel LyricsViewModel
        {
            get
            {
                EnsureInitialized();

                var viewModel = Container.GetInstance<LyricsViewModel>();
                viewModel.SetupLyricsCommand.Execute(new Audio());
                return viewModel;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "What? It is called!")]
        private static void EnsureInitialized()
        {
            LazyInitializer.EnsureInitialized(ref _wavePlayerApp, Initialize);
        }

        private static WavePlayerApp Initialize()
        {
            var player = Container.GetInstance<WavePlayerApp>();

            player.Initialize();

            return player;
        }
    }
}
