using WavePlayer.UI.ViewModels;

namespace WavePlayer.UI.Navigation
{
    internal class NavigationRule
    {
        public string TargetType { get; set; }

        public ViewModelBase ChildViewModel { get; set; }

        public HostViewModel HostViewModel { get; set; }

        public bool IsJournaled { get; set; }
    }
}
