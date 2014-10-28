using System.Collections.Generic;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels
{
    public class MainViewModel : HostViewModel
    {
        private readonly INavigationService _navigationService;
        
        public MainViewModel(INavigationService navigationService, PlayerViewModel playerView)
        {
            _navigationService = navigationService;
            PlayerView = playerView;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ProductNameString
        {
            get { return Resources.ProductName; }
        }

        public new PageViewModel CurrentView
        {
            get { return (PageViewModel)base.CurrentView; }

            set
            {
                base.CurrentView = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Hack: easy to bind to a view")]
        public PageViewModel SelectView
        {
            set
            {
                if (value != null &&
                    CurrentView != value)
                {
                    _navigationService.Navigate(value);
                }
            }
        }

        public IEnumerable<PageViewModel> Views { get; set; } 

        public PlayerViewModel PlayerView { get; private set; }
    }
}
