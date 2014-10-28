namespace WavePlayer.UI.ViewModels
{
    public class HostViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;

        public ViewModelBase CurrentView
        {
            get { return _currentView; }

            set { SetField(ref _currentView, value); }
        }
    }
}
