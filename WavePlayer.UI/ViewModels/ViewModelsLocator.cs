using System.Diagnostics;
using System.Linq;
using WavePlayer.UI.Media;

namespace WavePlayer.UI.ViewModels
{
    public class ViewModelsLocator
    {
        private PlayerViewModel _playerViewModel;
        private WelcomeViewModel _welcomeViewModel;

        public PlayerViewModel PlayerViewModel
        {
            get
            {
                if (_playerViewModel == null)
                {
                    Debug.WriteLine("Create player view mode");

                    var player = new WavePlayer.Media.Player(new PlayerEngine());
                    player.Engine.Volume = 0;
                    _playerViewModel = new PlayerViewModel(player);

                    //var trackList = WavePlayer.Media.FileSystem.TrackProvider.GetTrackList(@"G:\Music\Вася Обломов");

                    //player.PlayTrackList(trackList, trackList.First());
                }

                return _playerViewModel;
            }
        }

        public WelcomeViewModel WelcomeViewModel
        {
            get
            {
                if (_welcomeViewModel == null)
                {
                    Debug.WriteLine("Create welcome view model");

                   _welcomeViewModel = new WelcomeViewModel();
                }

                return _welcomeViewModel;
            }
        }
    }
}
