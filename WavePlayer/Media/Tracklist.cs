using System.Collections.Generic;

namespace WavePlayer.Media
{
    public class Tracklist<TTrack> : ITracklist where TTrack : Track
    {
        private readonly IList<TTrack> _tracks;

        public Tracklist(IList<TTrack> tracks)
        {
            _tracks = tracks;
        }

        public IList<TTrack> Source
        {
            get { return _tracks; }
        }

        public int Count
        {
            get { return _tracks.Count; }
        }

        public int IndexOf(Track item)
        {
            var track = item as TTrack;

            if (track == null)
            {
                return -1;
            }
            
           return _tracks.IndexOf(track);
        }

        public Track this[int index]
        {
            get
            {
                return _tracks[index];
            }
        }
    }
}
