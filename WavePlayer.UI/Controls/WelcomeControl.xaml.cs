using System.Windows;
using System.Windows.Controls;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.ViewModels;
using WavePlayer.UI.Windows;

namespace WavePlayer.UI.Controls
{
    /// <summary>
    /// Interaction logic for WelcomeControl.xaml
    /// </summary>
    public partial class WelcomeControl : UserControl
    {
        public WelcomeControl()
        {
            InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;

            var loginWindow = new LoginWindow()
            {
                Owner = app.MainWindow,
                DataContext = App.Container.GetInstance<LoginViewModel>()
            };

            loginWindow.Login();

            loginWindow.ShowDialog();

            if (loginWindow.FailureReason != null)
            {
                var dialogService = App.Container.GetInstance<IDialogService>();

                dialogService.NotifyError(loginWindow.FailureReason);
            }
        }
    }
}
