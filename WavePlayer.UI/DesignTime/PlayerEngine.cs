using System;
using WavePlayer.Media;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal class PlayerEngine : IPlayerEngine
    {
        private PlaybackState _state;

        public event System.EventHandler<Common.ExceptionEventArgs> MediaFailed
        {
            add { throw new NotSupportedException(); }
            remove { }
        }

        public event System.EventHandler MediaOpened
        {
            add { throw new NotSupportedException(); }
            remove { }
        }

        public event System.EventHandler MediaEnded
        {
            add { throw new NotSupportedException(); }
            remove { }
        }

        public event System.EventHandler BufferingStarted
        {
            add { throw new NotSupportedException(); }
            remove { }
        }

        public event System.EventHandler BufferingEnded
        {
            add { throw new NotSupportedException(); }
            remove { }
        }

        public event System.EventHandler PlaybackStateChanged;

        public PlaybackState PlaybackState { get{return _state;} set { _state = value; } }

        public System.Uri Source
        {
            get { return null; }
        }

        public double Volume { get; set; }

        public bool IsMuted { get; set; }

        public System.TimeSpan Position { get; set; }

        public System.TimeSpan Duration
        {
            get { return TimeSpan.Zero; }
        }

        public void Open(System.Uri source)
        {
            PlaybackState = PlaybackState.Opening;
            OnPlaybackStateChanged();
        }

        public void Play()
        {
           PlaybackState = PlaybackState.Playing;
            OnPlaybackStateChanged();
        }

        public void Pause()
        {
            PlaybackState = PlaybackState.Paused;
            OnPlaybackStateChanged();
        }

        public void Stop()
        {
            PlaybackState = PlaybackState.Stopped;
            OnPlaybackStateChanged();
        }

        public void Close()
        {
            PlaybackState = PlaybackState.Stopped;
            OnPlaybackStateChanged();
        }

        private void OnPlaybackStateChanged()
        {
            var temp = PlaybackStateChanged;

            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
    }
}

#endif
