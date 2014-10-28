using System;
using System.Diagnostics;

namespace WavePlayer.Media
{
    public class Player : IPlayer
    {
        private readonly IPlayerEngine _playerEngine;
        private readonly object _lock = new object();
        private readonly Random _random = new Random();

        public Player(IPlayerEngine playerEngine)
        {
            _playerEngine = playerEngine;
            _playerEngine.MediaOpened += PlayerMediaOpened;
            _playerEngine.MediaEnded += PlayerMediaEnded;
        }

        public IPlayerEngine Engine { get { return _playerEngine; } }

        public Track Track { get; private set; }

        public ITracklist Tracklist { get; private set; }

        public bool Loop { get; set; }

        public bool Shuffle { get; set; }

        public void PlayTracks(ITracklist tracklist, Track startTrack)
        {
            if (tracklist == null)
            {
                throw new ArgumentNullException("tracklist");
            }

            if (startTrack == null)
            {
                throw new ArgumentNullException("startTrack");
            }

            if (tracklist.IndexOf(startTrack) == -1)
            {
                throw new InvalidOperationException("Specified track list does not contain 'start' track");
            }

            lock (_lock)
            {
                Tracklist = tracklist;
                Open(startTrack);
            }
        }

        public bool CanForwardTrack()
        {
            return CanShuffle() || CanForwardTrackInternal();
        }

        public bool CanRewindTrack()
        {
            return CanShuffle() || CanRewindTrackInternal();
        }

        public void ForwardTrack()
        {
            lock (_lock)
            {
                Forward();
            }
        }

        public void RewindTrack()
        {
            lock (_lock)
            {
                Rewind();
            }
        }

        private void Open(Track newTrack)
        {
            var oldTrack = Track;

            if (oldTrack != null)
            {
                oldTrack.IsPlayingNow = false;
            }

            Track = newTrack;

            Track.IsPlayingNow = true;

            _playerEngine.Open(Track.Source);
        }

        private bool Forward()
        {
            try
            {
                if (CanShuffle())
                {
                    return Random();
                }

                if (CanForwardTrackInternal())
                {
                    var index = Tracklist.IndexOf(Track);

                    var track = Tracklist[index + 1];

                    Open(track);

                    return true;
                }

                Debug.WriteLine("Reached the end of the playlist or playlist is null");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.WriteLine("Failed to select next item in the playlist");
            }

            return false;
        }

        private bool Rewind()
        {
            try
            {
                if (CanShuffle())
                {
                    return Random();
                }

                if (CanRewindTrack())
                {
                    var index = Tracklist.IndexOf(Track);

                    var track = Tracklist[index - 1];

                    Open(track);

                    return true;
                }

                Debug.WriteLine("Rechead the start of the playlist or playlist is null");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.WriteLine("Failed to select previous item in the playlist");
            }

            return false;
        }

        private bool CanShuffle()
        {
            return Shuffle && Tracklist != null && Tracklist.Count != 0;
        }

        private bool Random()
        {
            var index = Track != null ? Tracklist.IndexOf(Track) : -1;

            var randomIndex = _random.Next(Tracklist.Count);

            if (randomIndex == index)
            {
                randomIndex += 1;
            }

            try
            {
                var track = Tracklist[randomIndex];

                Open(track);

                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.WriteLine("Failed to select random item in the playlist");
            }

            return false;
        }

        private bool CanForwardTrackInternal()
        {
            if (Tracklist == null ||
                Track == null)
            {
                return false;
            }

            var index = Tracklist.IndexOf(Track);

            return Tracklist.Count - 1 > index;
        }

        private bool CanRewindTrackInternal()
        {
            if (Tracklist == null ||
                Track == null)
            {
                return false;
            }

            var index = Tracklist.IndexOf(Track);

            return index > 0;
        }

        private void PlayerMediaOpened(object sender, EventArgs e)
        {
            Debug.WriteLine("Playing item from the source: {0}", _playerEngine.Source);
            _playerEngine.Play();
        }

        private void PlayerMediaEnded(object sender, EventArgs e)
        {
            Debug.WriteLine("Ended item playback from the source: {0}. Going to play the next item", _playerEngine.Source);

            lock (_lock)
            {
                if (Loop)
                {
                    _playerEngine.Stop();
                    _playerEngine.Play();
                }
                else if (!Forward())
                {
                    _playerEngine.Stop();
                }
            }
        }
    }
}
