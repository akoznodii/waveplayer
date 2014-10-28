using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Localization;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Themes;
using WavePlayer.Authorization;
using WavePlayer.Users;

namespace WavePlayer.UI.ViewModels
{
    public class SettingsViewModel : PageViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IThemeService _themeService;
        private readonly ILocalizationService _localizationService;
        private RelayCommand _signOutCommand;
        private RelayCommand _applyChangesCommand;
        private Theme _selecteTheme;
        private Accent _selecteAccent;
        private CultureInfo _selectedCulture;

        public SettingsViewModel(IAuthorizationService authorizationService, IThemeService themeService, ILocalizationService localizationService, INavigationService navigationService, IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _authorizationService = authorizationService;
            _themeService = themeService;
            _localizationService = localizationService;
        }

        public override string Title
        {
            get { return Resources.Settings; }
        }

        #region Localization fields
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string AccountString
        {
            get { return Resources.VkAccount; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ThemeString
        {
            get { return Resources.Theme; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string BackgroundString
        {
            get { return Resources.Background; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string AccentString
        {
            get { return Resources.Accent; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string LanguageString
        {
            get { return Resources.Language; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string SignOutString
        {
            get { return Resources.SignOut; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ApplyString
        {
            get { return Resources.Apply; }
        }
        #endregion

        public ICommand SignOutCommand
        {
            get
            {
                if (_signOutCommand == null)
                {
                    _signOutCommand = new RelayCommand(() => SingOutAsync());
                }

                return _signOutCommand;
            }
        }

        public ICommand ApplyChangesCommand
        {
            get
            {
                if (_applyChangesCommand == null)
                {
                    _applyChangesCommand = new RelayCommand(ApplyChanges, CanApplyChanges);
                }

                return _applyChangesCommand;
            }
        }

        public IEnumerable<Theme> AvailableThemes
        {
            get { return _themeService.AvailableThemes; }
        }

        public IEnumerable<Accent> AvailableAccents
        {
            get { return _themeService.AvailableAccents; }
        }

        public IEnumerable<CultureInfo> AvailableCultures
        {
            get { return _localizationService.AvailableCultures; }
        }

        public Theme CurrentTheme
        {
            get
            {
                return _themeService.CurrentTheme;
            }

            set
            {
                _selecteTheme = value;
            }
        }

        public Accent CurrentAccent
        {
            get
            {
                return _themeService.CurrentAccent;
            }

            set
            {
                _selecteAccent = value;
            }
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                return _localizationService.CurrentCulture;
            }

            set
            {
                _selectedCulture = value;
            }
        }

        public User CurrentUser
        {
            get
            {
                return _authorizationService.CurrentUser;
            }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();

            RaisePropertyChanged("AccountString");
            RaisePropertyChanged("ThemeString");
            RaisePropertyChanged("BackgroundString");
            RaisePropertyChanged("AccentString");
            RaisePropertyChanged("LanguageString");
            RaisePropertyChanged("SignOutString");
            RaisePropertyChanged("ApplyString");
        }

        private Task SingOutAsync()
        {
            return Task.Factory.StartNew(SingOut);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Show error message to the end-user")]
        private void SingOut()
        {
            SafeExecute(() =>
            {
                DialogService.StartProgress();

                _authorizationService.Logout();

                DialogService.StopProgress();

                NavigationService.Navigate<LoginViewModel>();
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Show error message to the end-user")]
        private void ApplyChanges()
        {
            SafeExecute(() =>
               {
                   if (_selecteTheme == null)
                   {
                       _selecteTheme = CurrentTheme;
                   }

                   if (_selecteAccent == null)
                   {
                       _selecteAccent = CurrentAccent;
                   }

                   if (_selecteTheme.Name != CurrentTheme.Name ||
                       _selecteAccent.Name != CurrentTheme.Name)
                   {
                       _themeService.ChangeTheme(_selecteTheme.Name, _selecteAccent.Name);
                   }

                   if (_selectedCulture != null && _selectedCulture.LCID != CurrentCulture.LCID)
                   {
                       _localizationService.SetCurrentCulture(_selectedCulture.LCID);
                   }
               });
        }

        private bool CanApplyChanges()
        {
            return (_selecteAccent != null && _selecteAccent.Name != CurrentAccent.Name) ||
                   (_selecteTheme != null && _selecteTheme.Name != CurrentTheme.Name) ||
                   (_selectedCulture != null && CurrentCulture.LCID != _selectedCulture.LCID);
        }
    }
}
