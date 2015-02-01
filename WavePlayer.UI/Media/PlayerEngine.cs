using System;
using System.Diagnostics;
using System.Windows.Media;
using WavePlayer.Media;
using ExceptionEventArgs = WavePlayer.Common.ExceptionEventArgs;

namespace WavePlayer.UI.Media
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "IoC container do it")]
    internal class PlayerEngine : IPlayerEngine
    {
        private readonly MediaPlayer _mediaPlayer;
        private PlaybackState _state;

        public PlayerEngine()
        {
            _mediaPlayer = new MediaPlayer() { ScrubbingEnabled = true };
            _mediaPlayer.MediaFailed += MediaPlayerMediaFailed;
        }
        
        public event EventHandler<ExceptionEventArgs> MediaFailed;

        public event EventHandler MediaOpened
        {
            add { _mediaPlayer.MediaOpened += value; }
            remove { _mediaPlayer.MediaOpened -= value; }
        }

        public event EventHandler MediaEnded
        {
            add { _mediaPlayer.MediaEnded += value; }
            remove { _mediaPlayer.MediaEnded -= value; }
        }

        public event EventHandler BufferingStarted
        {
            add { _mediaPlayer.BufferingStarted += value; }
            remove { _mediaPlayer.BufferingStarted -= value; }
        }

        public event EventHandler BufferingEnded
        {
            add { _mediaPlayer.BufferingEnded += value; }
            remove { _mediaPlayer.BufferingEnded -= value; }
        }

        public event EventHandler PlaybackStateChanged;

        public PlaybackState PlaybackState
        {
            get
            {
                return _state;
            }

            private set
            {
                _state = value;
                OnPlaybackStateChanged();
            }
        }

        public IEqualizer Equalizer
        {
            get
            {
                return null;
            }
        }

        public Uri Source
        {
            get { return _mediaPlayer.Source; }
        }

        public double Volume
        {
            get
            {
                return _mediaPlayer.Volume;
            }

            set
            {
                _mediaPlayer.Volume = value;
            }
        }

        public bool IsMuted
        {
            get
            {
                return _mediaPlayer.IsMuted;
            }

            set
            {
                _mediaPlayer.IsMuted = value;
            }
        }

        public TimeSpan Position
        {
            get
            {
                return _mediaPlayer.Position;
            }

            set
            {
                _mediaPlayer.Position = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return _mediaPlayer.NaturalDuration.HasTimeSpan ? _mediaPlayer.NaturalDuration.TimeSpan : TimeSpan.Zero;
            }
        }

        public void Open(Uri source)
        {
            Close();

            Debug.WriteLine("Opening media source: {0}", source);
            _mediaPlayer.Open(source);

            PlaybackState = PlaybackState.Opening;
        }

        public void Play()
        {
            Debug.WriteLine("Playing media source: {0}", Source);
            _mediaPlayer.Play();

            PlaybackState = PlaybackState.Playing;
        }

        public void Pause()
        {
            Debug.WriteLine("Pausing media source: {0}", Source);

            _mediaPlayer.Pause();

            PlaybackState = PlaybackState.Paused;
        }

        public void Stop()
        {
            Debug.WriteLine("Stopping media source: {0}", Source);
            _mediaPlayer.Stop();
            
            PlaybackState = PlaybackState.Stopped;
        }

        public void Close()
        {
            var volume = _mediaPlayer.Volume;
            var isMuted = _mediaPlayer.IsMuted;

            Debug.WriteLine("Closing media source: {0}", Source);

            _mediaPlayer.Close();
            _mediaPlayer.Volume = volume;
            _mediaPlayer.IsMuted = isMuted;

            PlaybackState = PlaybackState.Stopped;
        }

        private void MediaPlayerMediaFailed(object sender, System.Windows.Media.ExceptionEventArgs e)
        {
            PlaybackState = PlaybackState.Stopped;

            var handler = MediaFailed;

            if (handler != null)
            {
                handler(this, new WavePlayer.Common.ExceptionEventArgs() { ErrorOccurred = e.ErrorException });
            }
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
