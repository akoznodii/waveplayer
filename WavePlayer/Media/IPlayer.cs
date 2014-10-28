namespace WavePlayer.Media
{
    public interface IPlayer
    {
        IPlayerEngine Engine { get; }

        Track Track { get; }

        ITracklist Tracklist { get; }

        bool Loop { get; set; }

        bool Shuffle { get; set; }

        void PlayTracks(ITracklist tracklist, Track startTrack);

        bool CanForwardTrack();

        bool CanRewindTrack();

        void ForwardTrack();

        void RewindTrack();
    }
}