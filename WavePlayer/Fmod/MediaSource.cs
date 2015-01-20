using System;
using System.Diagnostics;
using System.IO;
using WavePlayer.Network;
using FileInfo = WavePlayer.Network.FileInfo;

namespace WavePlayer.Fmod
{
    internal sealed class MediaSource : IDisposable
    {
        private readonly object _lock = new object();
        private readonly object _downloaderLock = new object();
        private readonly FileInfo _fileInfo;
        
        private Downloader _downloader;
        private Stream _remoteStream;
        private BufferedStream _bufferedStream;
        private MemoryStream _memoryStream;
        private bool _canReadFromMemory;

        public MediaSource(FileInfo fileInfo, int bufferSize, bool cacheInMemory)
        {
            _fileInfo = fileInfo;
            BufferSize = bufferSize;

            if (cacheInMemory)
            {
                _downloader = new Downloader(_fileInfo, bufferSize, 4, 5, DownloadCallback);
                _memoryStream = new MemoryStream((int)_fileInfo.ContentLength);
            }
        }

        public int BufferSize { get; private set; }

        public bool IsOpened { get { return _remoteStream != null; } }

        public Uri Location { get { return _fileInfo.Location; } }

        public long Length { get { return _fileInfo.ContentLength; } }

        public long Position { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public void Open()
        {
            Open(0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public void Open(long position)
        {
            lock (_lock)
            {
                CheckDownloaderState();

                if (_canReadFromMemory)
                {
                    return;
                }

                OpenRemoteInternal(position);

                Position = position;
            }
        }
        
        public void Close(bool freeCache)
        {
            lock (_lock)
            {
                if (freeCache)
                {
                    DisposeDownloader();
                    CloseMemoryCache();
                }

                CloseRemoteInternal();
            }
        }

        public void Seek(long position)
        {
            lock (_lock)
            {
                CheckDownloaderState();

                if (_canReadFromMemory)
                {
                    _memoryStream.Seek(position, SeekOrigin.Begin);
                }
                else
                {
                    if (IsOpened && Position == position)
                    {
                        return;
                    }

                    CloseRemoteInternal();
                    OpenRemoteInternal(position);
                }

                Position = position;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                CheckDownloaderState();

                Stream stream = null;

                if (_canReadFromMemory)
                {
                    stream = _memoryStream;
                }
                else if (IsOpened)
                {
                    stream = _bufferedStream;
                }

                if (stream == null)
                {
                    return 0;
                }

                var readCount = stream.Read(buffer, offset, count);

                Position += readCount;
                
                return readCount;
            }
        }

        public void Dispose()
        {
            Close(true);
        }

        private void CloseRemoteInternal()
        {
            if (!IsOpened)
            {
                return;
            }

            _bufferedStream.Dispose();
            _bufferedStream = null;

            _remoteStream.Dispose();
            _remoteStream = null;
        }

        private void OpenRemoteInternal(long position)
        {
            CloseRemoteInternal();

            _remoteStream = HttpWebRequestHelper.CreateStream(_fileInfo.Location, position, _fileInfo.ContentLength);

            _bufferedStream = new BufferedStream(_remoteStream, BufferSize);
        }

        private void DownloadCallback(byte[] buffer, long position, int count)
        {
            lock (_downloaderLock)
            {
                _memoryStream.Seek(position, SeekOrigin.Begin);
                _memoryStream.Write(buffer, 0, count);
            }
        }
        
        private void CheckDownloaderState()
        {
            if (_downloader == null)
            {
                return;
            }

            if (_downloader.State == DownloaderState.Failed || 
                _downloader.State == DownloaderState.Ready)
            {
                Debug.WriteLine("Download started");
                _downloader.Start();
            }
            else if (_downloader.State == DownloaderState.Completed)
            {
                Debug.WriteLine("Download complete");
                DisposeDownloader();

                _memoryStream.Seek(Position, SeekOrigin.Begin);

                _canReadFromMemory = true;

                CloseRemoteInternal();
            }
        }

        private void DisposeDownloader()
        {
            if (_downloader == null)
            {
                return;
            }

            if (_downloader.State == DownloaderState.Loading)
            {
                _downloader.Cancel();
            }

            _downloader.Dispose();
            _downloader = null;
        }

        private void CloseMemoryCache()
        {
            if (_memoryStream == null)
            {
                return;
            }

            _memoryStream.Dispose();
            _memoryStream = null;
        }
    }
}
