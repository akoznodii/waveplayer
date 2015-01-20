using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WavePlayer.Common;
using WavePlayer.Fmod.Native;
using WavePlayer.Media;

namespace WavePlayer.Fmod
{
    public sealed class PlayerEngine : IPlayerEngine, IDisposable
    {
        private const int BufferSize = 8192;
        private const long MaximumCacheSize = 25 * 1024 * 1024;

        private readonly object _engineLock = new object();
        private FmodSystem _fmodSystem;
        private Sound _sound;
        private Channel _channel;
        private Timer _timer;

        private ChannelControlCallback _channelControlCallback;
        private OpenCallback _openCallback;
        private CloseCallback _closeCallback;
        private ReadCallback _readCallback;
        private SeekCallback _seekCallback;

        private MediaSource _mediaSource;

        private byte[] _buffer;

        private PlaybackState _playbackState;
        private double _volume;
        private bool _isMuted;

        public PlayerEngine()
        {
            Initialize();
        }

        public event EventHandler<ExceptionEventArgs> MediaFailed;

        public event EventHandler MediaOpened;

        public event EventHandler MediaEnded;

        public event EventHandler BufferingStarted;

        public event EventHandler BufferingEnded;

        public event EventHandler PlaybackStateChanged;

        public PlaybackState PlaybackState
        {
            get { return _playbackState; }

            set
            {
                _playbackState = value;
                OnEvent(PlaybackStateChanged);
            }
        }

        public Uri Source { get; private set; }

        public double Volume
        {
            get
            {
                return _volume;
            }

            set
            {
                if (Math.Abs(_volume - value) <= float.Epsilon)
                {
                    return;
                }

                lock (_engineLock)
                {
                    using (var channelGroup = _fmodSystem.MasterChannelGroup)
                    {
                        channelGroup.Volume = (float)value;
                    }

                    _fmodSystem.Update();

                    _volume = value;
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                return _isMuted;
            }

            set
            {
                if (_isMuted == value)
                {
                    return;
                }

                lock (_engineLock)
                {
                    using (var channelGroup = _fmodSystem.MasterChannelGroup)
                    {
                        channelGroup.Mute = value;
                    }

                    _fmodSystem.Update();

                    _isMuted = value;
                }
            }
        }

        public TimeSpan Position
        {
            get
            {
                lock (_engineLock)
                {
                    if (_channel != null)
                    {
                        return IsSeeking ? SeekPosition : TimeSpan.FromMilliseconds(_channel.Position);
                    }
                }

                return TimeSpan.Zero;
            }

            set
            {
                lock (_engineLock)
                {
                    if (_channel != null)
                    {
                        var paused = _channel.Paused;

                        _channel.Paused = true;

                        _channel.Position = (uint)value.TotalMilliseconds;
                        IsSeeking = true;
                        SeekPosition = value;
                        _fmodSystem.Update();

                        _channel.Paused = paused;
                    }
                }
            }
        }

        public TimeSpan Duration { get; private set; }

        private bool IsStarving { get; set; }

        private bool IsBuffering { get; set; }

        private bool IsSeeking { get; set; }

        private TimeSpan SeekPosition { get; set; }

        public void Initialize()
        {
            _fmodSystem = new FmodSystem();
            _fmodSystem.Initialize(1, InitFlags.FMOD_INIT_NORMAL);
            _channelControlCallback = ChannelCallback;

            _openCallback = OpenCallback;
            _closeCallback = CloseCallback;
            _readCallback = ReadCallback;
            _seekCallback = SeekCallback;

            _buffer = new byte[BufferSize];

            _fmodSystem.SetBufferSize(BufferSize);

            _fmodSystem.SetFileSystem(_openCallback, _closeCallback, _readCallback, _seekCallback);

            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.Zero);

            using (var channelGroup = _fmodSystem.MasterChannelGroup)
            {
                _volume = channelGroup.Volume;
                _isMuted = channelGroup.Mute;
            }
        }

        public void Open(Uri source)
        {
            lock (_engineLock)
            {
                OpenInternal(source);
            }

            PlaybackState = PlaybackState.Opening;
        }

        public void Play()
        {
            lock (_engineLock)
            {
                if (_channel == null && Source != null)
                {
                    OpenInternal(Source);
                    return;
                }

                if (_channel == null || !_channel.Paused)
                {
                    return;
                }

                _channel.Paused = false;
                _fmodSystem.Update();
            }

            PlaybackState = PlaybackState.Playing;
        }

        public void Pause()
        {
            lock (_engineLock)
            {
                if (_channel == null || _channel.Paused)
                {
                    return;
                }

                _channel.Paused = true;
                _fmodSystem.Update();
            }

            PlaybackState = PlaybackState.Paused;
        }

        public void Stop()
        {
            lock (_engineLock)
            {
                StopInternal();
            }

            PlaybackState = PlaybackState.Stopped;
        }

        public void Close()
        {
            lock (_engineLock)
            {
                CloseInternal();
            }
        }

        public void Dispose()
        {
            CloseInternal();

            if (_fmodSystem != null)
            {
                _fmodSystem.Dispose();
                _fmodSystem = null;
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (_mediaSource != null)
            {
                _mediaSource.Close(true);
                _mediaSource = null;
            }
        }

        private void OpenInternal(Uri source)
        {
            CloseInternal();

            var soundMode = SoundMode.FMOD_CREATESTREAM | SoundMode.FMOD_NONBLOCKING;

            _sound = _fmodSystem.CreateSound(source.ToString(), soundMode);
            Source = source;

            _timer.Change(TimeSpan.FromMilliseconds(500), TimeSpan.Zero);
        }

        private void PlayInternal()
        {
            Duration = TimeSpan.FromMilliseconds(_sound.Length);
            _channel = _fmodSystem.PlaySound(_sound, true);
            _channel.SetCallback(_channelControlCallback);
        }

        private void StopInternal()
        {
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }

            if (_sound != null)
            {
                _sound.Dispose();
                _sound = null;
            }

            IsBuffering = false;
            IsStarving = false;
            IsSeeking = false;
            SeekPosition = TimeSpan.Zero;
        }

        private void CloseInternal()
        {
            StopInternal();

            Duration = TimeSpan.Zero;
            Source = null;

            _timer.Change(TimeSpan.Zero, TimeSpan.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is Okay")]
        private void TimerCallback(object state)
        {
            try
            {
                lock (_engineLock)
                {
                    UpdateEngineState();

                    if (Source != null)
                    {
                        _timer.Change(TimeSpan.FromMilliseconds(500), TimeSpan.Zero);
                    }
                }
            }
            catch (Exception error)
            {
                OnErrorAsync(error);
            }
        }

        private void UpdateEngineState()
        {
            _fmodSystem.Update();

            if (_sound == null)
            {
                return;
            }

            var soundState = _sound.State;

            if (soundState.OpenState == OpenState.FMOD_OPENSTATE_READY)
            {
                PlayInternal();
                OnEventAsync(MediaOpened);
                return;
            }

            if (soundState.IsStarving)
            {
                Debug.WriteLine("Starving...");
                IsStarving = true;
                _channel.Mute = true;
            }
            else if (IsStarving)
            {
                Debug.WriteLine("Done starving...");
                IsStarving = false;
                _channel.Mute = false;
            }

            if (soundState.OpenState == OpenState.FMOD_OPENSTATE_BUFFERING)
            {
                IsBuffering = true;
                OnEventAsync(BufferingStarted);
            }
            else if (IsBuffering)
            {
                IsBuffering = false;
                OnEventAsync(BufferingEnded);
            }

            if (IsSeeking && soundState.OpenState != OpenState.FMOD_OPENSTATE_SETPOSITION)
            {
                IsSeeking = false;
                SeekPosition = TimeSpan.Zero;
            }

            if (soundState.OpenState == OpenState.FMOD_OPENSTATE_ERROR)
            {
                OnErrorAsync();
            }
        }

        private ErrorCode ChannelCallback(IntPtr channelHandle, ChannelType channelType, ChannelCallbackType callbackType, IntPtr commandData1, IntPtr commandData2)
        {
            if (callbackType == ChannelCallbackType.FMOD_CHANNELCONTROL_CALLBACK_END && _channel != null)
            {
                OnEventAsync(MediaEnded);
            }

            return ErrorCode.FMOD_OK;
        }

        private void OnEvent(EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void OnEventAsync(EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler.BeginInvoke(this, EventArgs.Empty, null, null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is Okay")]
        private void OnErrorAsync(Exception error = null)
        {
            Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("Fmod Player Engine has failed with error:{0}{1}", Environment.NewLine, error);

                try
                {
                    CloseInternal();

                    PlaybackState = PlaybackState.Stopped;

                    var eventHandler = MediaFailed;

                    if (eventHandler != null)
                    {
                        eventHandler.Invoke(this, new ExceptionEventArgs() { ErrorOccurred = error });
                    }
                }
                catch
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
            });
        }

        private ErrorCode SeekCallback(IntPtr handle, int position, IntPtr userData)
        {
            return HandleException(() =>
            {
                if (!CheckMediaSource())
                {
                    return ErrorCode.FMOD_ERR_FILE_NOTFOUND;
                }

                _mediaSource.Seek(position);

                return ErrorCode.FMOD_OK;
            });
        }

        private ErrorCode ReadCallback(IntPtr handle, IntPtr buffer, uint sizeBytes, ref uint bytesRead, IntPtr userData)
        {
            var totalRead = 0;

            var result = HandleException(() =>
            {
                if (!CheckMediaSource())
                {
                    return ErrorCode.FMOD_ERR_FILE_NOTFOUND;
                }

                var requestedLength = (int)sizeBytes;

                while (totalRead < requestedLength)
                {
                    var length = Math.Min(_buffer.Length, requestedLength - totalRead);

                    if (_mediaSource.Position + length > _mediaSource.Length)
                    {
                        length = (int)(_mediaSource.Length - _mediaSource.Position);
                    }

                    if (length == 0)
                    {
                        break;
                    }

                    var count = _mediaSource.Read(_buffer, 0, length);

                    Marshal.Copy(_buffer, 0, buffer, count);

                    totalRead += count;
                }

                return sizeBytes == totalRead ? ErrorCode.FMOD_OK : ErrorCode.FMOD_ERR_FILE_EOF;
            });

            bytesRead = (uint)totalRead;

            return result;
        }

        private ErrorCode CloseCallback(IntPtr handle, IntPtr userData)
        {
            return HandleException(() =>
            {
                if (!CheckMediaSource())
                {
                    return ErrorCode.FMOD_ERR_FILE_NOTFOUND;
                }

                if (_mediaSource.IsOpened)
                {
                    _mediaSource.Close(false);
                }

                return ErrorCode.FMOD_OK;
            });
        }

        private ErrorCode OpenCallback(string fileName, ref uint fileSize, ref IntPtr handle, ref IntPtr userData)
        {
            var length = 0U;

            var result = HandleException(() =>
            {
                var location = new Uri(fileName);

                var newLocation = _mediaSource == null || _mediaSource.Location != location;

                if (_mediaSource != null)
                {
                    _mediaSource.Close(newLocation);
                }

                if (newLocation)
                {
                    var fileInfo = Network.HttpWebRequestHelper.GetRemoteFileInfo(location);

                    var cacheMediaFile = fileInfo.ContentLength <= MaximumCacheSize;
                    
                    Debug.WriteLine("Media source cache mode enabled: {0}, size: {1}", cacheMediaFile, fileInfo.ContentLength);

                    _mediaSource = new MediaSource(fileInfo, BufferSize * 8, cacheMediaFile);
                }

                length = (uint)_mediaSource.Length;

                return ErrorCode.FMOD_OK;
            });

            fileSize = length;

            return result;
        }

        private bool CheckMediaSource()
        {
            return _mediaSource != null &&
                   _mediaSource.Location == Source;
        }

        private static ErrorCode HandleException(Func<ErrorCode> func)
        {
            try
            {
                return func();
            }
            catch (WebException webException)
            {
                switch (webException.Status)
                {
                    case WebExceptionStatus.ConnectionClosed:
                        return ErrorCode.FMOD_ERR_NET_CONNECT;
                    case WebExceptionStatus.Timeout:
                        return ErrorCode.FMOD_ERR_HTTP_TIMEOUT;
                    default:
                        return ErrorCode.FMOD_ERR_HTTP;
                }
            }
        }
    }
}
