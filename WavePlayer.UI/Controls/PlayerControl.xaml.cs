using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WavePlayer.Media;
using WavePlayer.UI.ViewModels;

namespace WavePlayer.UI.Controls
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            InitializeComponent();
        }

        private void OnTransitioningTargetUpdated(object sender, DataTransferEventArgs e)
        {
            TrackTransitioningControl.ReloadTransition();
        }

        private void PlayerControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var newViewModel = e.NewValue as PlayerViewModel;

            if (newViewModel != null)
            {
                newViewModel.PropertyChanged += PlayerViewModelPropertyChanged;
            }

            var oldViewModel = e.OldValue as PlayerViewModel;

            if (oldViewModel != null)
            {
                oldViewModel.PropertyChanged -= PlayerViewModelPropertyChanged;
            }
        }

        private void PlayerViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var viewModel = sender as PlayerViewModel;

            if (viewModel != null &&
                args.PropertyName == "PlaybackState" &&
                viewModel.PlaybackState != PlaybackState.Playing &&
                viewModel.PlaybackState != PlaybackState.Paused)
            {
                TimeTransitioningControl.InvokeIfRequired(TimeTransitioningControl.ReloadTransition);
                LeftTimeTransitioningControl.InvokeIfRequired(LeftTimeTransitioningControl.ReloadTransition);
            }
        }
    }
}
