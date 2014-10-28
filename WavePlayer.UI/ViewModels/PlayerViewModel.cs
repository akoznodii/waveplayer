using System;
using System.Windows.Input;
using System.Windows.Threading;
using WavePlayer.Media;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Threading;

namespace WavePlayer.UI.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly IPlayer _player;
        private readonly IPlayerEngine _playerEngine;
        private readonly DispatcherTimer _timer;
        private readonly RelayCommand _forwardCommand;
        private readonly RelayCommand _rewindCommand;
        private readonly RelayCommand _playCommand;
        private Track _track;
        private PlaybackState _playbackState;
        private double _position;
        private double _duration;
        private bool _soundMuted;
        private bool _shuffle;
        private bool _loop;
        private double _soundLevel;

        public PlayerViewModel(IPlayer player)
        {
            _player = player;
            _playerEngine = _player.Engine;

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            _timer.Tick += OnTimerTick;
            _playerEngine.PlaybackStateChanged += OnPlaybackStateChanged;

            SetField(ref _soundMuted, _playerEngine.IsMuted, "SoundMuted");
            SetField(ref _soundLevel, _playerEngine.Volume * 100, "SoundLevel");

            _playCommand = new RelayCommand(() =>
            {
                if (_playerEngine == null)
                {
                    return;
                }

                if (_playerEngine.PlaybackState == PlaybackState.Playing)
                {
                    _playerEngine.Pause();
                }
                else
                {
                    _playerEngine.Play();
                }
            },
            () => Track != null);

            _forwardCommand = new RelayCommand(() =>
            {
                if (_player == null)
                {
                    return;
                }

                _player.ForwardTrack();
            },
            () => _player != null && _player.CanForwardTrack());

            _rewindCommand = new RelayCommand(() =>
            {
                if (_player == null)
                {
                    return;
                }

                _player.RewindTrack();
            },
            () => _player != null && _player.CanRewindTrack());
        }

        public PlaybackState PlaybackState { get { return _playbackState; } private set { SetField(ref _playbackState, value); } }

        public double Position
        {
            get
            {
                return _position;
            }

            set
            {
                if (_playerEngine != null)
                {
                    _playerEngine.Position = TimeSpan.FromMilliseconds(value);
                }

                SetField(ref _position, value);
            }
        }

        public double Duration { get { return _duration; } private set { SetField(ref _duration, value); } }

        public Track Track { get { return _track; } private set { SetField(ref _track, value); } }

        public bool SoundMuted
        {
            get
            {
                return _soundMuted;
            }

            set
            {
                if (_playerEngine != null)
                {
                    _playerEngine.IsMuted = value;
                }

                SetField(ref _soundMuted, value);
            }
        }

        public double SoundLevel
        {
            get
            {
                return _soundLevel;
            }

            set
            {
                if (_playerEngine != null)
                {
                    _playerEngine.Volume = value * 0.01;
                }

                SetField(ref _soundLevel, value);
            }
        }

        public bool Shuffle
        {
            get
            {
                return _shuffle;
            }

            set
            {
                if (_player != null)
                {
                    _player.Shuffle = value;
                }

                SetField(ref _shuffle, value);
            }
        }

        public bool Loop
        {
            get
            {
                return _loop;
            }

            set
            {
                if (_player != null)
                {
                    _player.Loop = value;
                }

                SetField(ref _loop, value);
            }
        }

        public ICommand PlayCommand
        {
            get
            {
                return _playCommand;
            }
        }

        public ICommand ForwardCommand
        {
            get
            {
                return _forwardCommand;
            }
        }

        public ICommand RewindCommand
        {
            get
            {
                return _rewindCommand;
            }
        }

        private void OnPlaybackStateChanged(object sender, EventArgs args)
        {
            var state = _playerEngine.PlaybackState;

            switch (state)
            {
                case PlaybackState.Stopped:
                    DispatcherHelper.InvokeOnUI(StopPlayback);
                    break;
                case PlaybackState.Opening:
                    Track = _player.Track;
                    break;
                case PlaybackState.Playing:
                    DispatcherHelper.InvokeOnUI(StartPlayback);
                    break;
            }

            PlaybackState = state;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (!_timer.IsEnabled)
            {
                return;
            }

            UpdatePlaybackPosition();
        }

        private void StopPlayback()
        {
            _timer.Stop();

            SetField(ref _position, 0, "Position");
        }

        private void StartPlayback()
        {
            UpdatePlaybackPosition();
            _timer.Start();
        }

        private void UpdatePlaybackPosition()
        {
            var postion = _playerEngine.Position;
            var duration = _playerEngine.Duration;

            SetField(ref _position, postion.TotalMilliseconds, "Position");
            Duration = duration.TotalMilliseconds;
        }
    }
}
