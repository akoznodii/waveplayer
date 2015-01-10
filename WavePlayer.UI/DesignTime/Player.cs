using System.Linq;
using WavePlayer.Audios;
using WavePlayer.Media;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal class Player : IPlayer
    {
        private readonly IPlayerEngine _playerEngine;
        private readonly Tracklist<Audio> _tracklist = new Tracklist<Audio>(DesignData.VkDataProvider.GetRecommendedAudios(null, false).ToList());
        
        public Player(IPlayerEngine playerEngine)
        {
            _playerEngine = playerEngine;
        }

        public IPlayerEngine Engine { get { return _playerEngine; } }

        public Track Track
        {
            get { return _tracklist.Source.First(); }
        }

        public ITracklist Tracklist
        {
            get { return _tracklist; }
        }

        public bool Loop
        {
            get;
            set;
        }

        public bool Shuffle
        {
            get;
            set;
        }

        public void PlayTracks(ITracklist tracklist, Track startTrack)
        {
        }

        public bool CanForwardTrack()
        {
            return true;
        }

        public bool CanRewindTrack()
        {
            return true;
        }

        public void ForwardTrack()
        {
        }

        public void RewindTrack()
        {
        }
    }
}
#endif
