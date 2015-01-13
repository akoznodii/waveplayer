using System;
using System.Windows;
using WavePlayer.Ioc;
using WavePlayer.UI.ViewModels;
using WavePlayer.UI.Windows;

namespace WavePlayer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Lazy<IContainer> IoC = new Lazy<IContainer>(IoCBootstrapper.GetContainer, true);

        public static IContainer Container { get { return IoC.Value; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            var player = Container.GetInstance<WavePlayerApp>();

            player.Initialize();
            
            DispatcherUnhandledException += player.OnUnhandledException;

            base.OnStartup(e);

            var mainViewModel = Container.GetInstance<HostViewModel>();
            var mainWindow = Container.GetInstance<HostWindow>();
            mainWindow.DataContext = mainViewModel;

            player.Start();

            mainWindow.Show();
        }
    }
}
