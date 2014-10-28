using System;
using WavePlayer.Common;

namespace WavePlayer.Media
{
    public class Track : ModelBase
    {
        private string _title;
        private string _artist;
        private bool _isPlayingNow;
        private TimeSpan _duration;
        private Uri _source;

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                SetField(ref _title, value);
            }
        }

        public string Artist
        {
            get
            {
                return _artist;
            }

            set
            {
                SetField(ref _artist, value);
            }
        }

        public bool IsPlayingNow
        {
            get
            {
                return _isPlayingNow;
            }

            set
            {
                SetField(ref _isPlayingNow, value);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                SetField(ref _duration, value);
            }
        }

        public Uri Source
        {
            get
            {
                return _source;
            }

            set
            {
                SetField(ref _source, value);
            }
        }
    }
}
