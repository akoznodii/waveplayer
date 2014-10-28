using System;
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using WavePlayer.UI.ViewModels;

namespace WavePlayer.UI.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();

            Title = Properties.Resources.AuthorizationWindow;
        }

        public Exception FailureReason { get; private set; }

        private LoginViewModel ViewModel { get; set; }

        public void Login()
        {
            var uri = ViewModel.LoginUri;

            WebBrowser.Source = uri;
        }
        
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty &&
                e.OldValue == null &&
                e.NewValue != null)
            {
                ViewModel = e.NewValue as LoginViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "I need to catch all exceptions")]
        private void WebBrowserOnNavigated(object sender, NavigationEventArgs e)
        {
            if (ViewModel == null ||
                !ViewModel.CanRetrieveToken(e.Uri))
            {
                return;
            }

            try
            {
                ViewModel.RetrieveToken(e.Uri);
                DialogResult = true;
            }
            catch (Exception exception)
            {
                FailureReason = exception;
            }

            Close();
        }
    }
}
