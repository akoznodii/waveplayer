using WavePlayer.UI.ViewModels;

namespace WavePlayer.UI.Navigation
{
    public interface INavigationService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As design")]
        void Navigate<TViewModel>() where TViewModel : ViewModelBase;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As design")]
        void Navigate<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        void Navigate<TViewModel>(TViewModel viewModel, object parameter = null) where TViewModel : ViewModelBase;

        void NavigateBack();

        bool CanNavigateBack();

        void SetupNavigationRule(HostViewModel hostViewModel, ViewModelBase viewModel, bool isJournaled = false);
    }
}
