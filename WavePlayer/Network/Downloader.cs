using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WavePlayer.Network
{
    internal sealed class Downloader : IDisposable
    {
        private readonly object _lock = new object();
        private bool _disposed;

        public Downloader(FileInfo info, int bufferSize, int segmentsCount, int maximumRetries, Action<byte[], long, int> chunkCallback)
        {
            FileInfo = info;
            ChunkCallback = chunkCallback;
            SegmentsCount = segmentsCount;
            MaximumRetriesCount = maximumRetries;
            Segments = new List<Segment>();
            BufferSize = bufferSize;
        }

        ~Downloader()
        {
            Dispose(false);
        }

        public event EventHandler<EventArgs> DownloadFailed;

        public event EventHandler<EventArgs> DownloadCompleted;

        public FileInfo FileInfo { get; private set; }

        public DownloaderState State { get; private set; }

        private int SegmentsCount { get; set; }

        private Action<byte[], long, int> ChunkCallback { get; set; }

        private CancellationTokenSource TokenSource { get; set; }

        private int MaximumRetriesCount { get; set; }

        private int RetriesCount { get; set; }

        private Task[] SegmentTasks { get; set; }

        private List<Segment> Segments { get; set; }

        private int BufferSize { get; set; }

        public void Cancel()
        {
            lock (_lock)
            {
                if (TokenSource != null)
                {
                    TokenSource.Cancel();
                }
            }
        }

        public void Start()
        {
            RestartInternal(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void RestartInternal(bool reset = false)
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("Downloader object is already disposed");
                }

                DisposeCancellationTokenSource();

                if (reset)
                {
                    RetriesCount = 0;
                }

                InitializeSegments();

                StartSegments();
            }
        }

        private void InitializeSegments()
        {
            if (!Segments.Any())
            {
                var count = FileInfo.AcceptRanges ? SegmentsCount : 1;

                var startPosistion = 0L;
                var segmentLength = FileInfo.ContentLength / count;

                for (var idx = 0; idx < count; idx++)
                {
                    var lastSegment = idx == count - 1;
                    var segment = new Segment()
                    {
                        Start = startPosistion,
                        End = lastSegment ? FileInfo.ContentLength : (startPosistion + segmentLength)
                    };

                    startPosistion = segment.End;

                    if (segment.Start != segment.End)
                    {
                        Segments.Add(segment);
                    }
                }
            }

            foreach (var segment in Segments.Where(s => s.State != DownloaderState.Completed))
            {
                segment.State = DownloaderState.Ready;
            }
        }

        private void StartSegments()
        {
            State = DownloaderState.Loading;

            RetriesCount++;

            TokenSource = new CancellationTokenSource();

            var segments = Segments.Where(s => s.State == DownloaderState.Ready).ToList();

            var segmentTasks = new Task[segments.Count];

            var token = TokenSource.Token;

            for (var idx = 0; idx < segments.Count; idx++)
            {
                var segment = segments[idx];
                segmentTasks[idx] = Task.Factory.StartNew(() => StartSegment(FileInfo, segment, ChunkCallback, token), token);
            }

            SegmentTasks = segmentTasks;

            Task.Factory.ContinueWhenAll(SegmentTasks, (tasks) =>
            {
                if (Segments.Any(s => s.State == DownloaderState.Failed))
                {
                    if (RetriesCount == MaximumRetriesCount)
                    {
                        State = DownloaderState.Failed;
                        RaiseEvent(DownloadFailed);
                    }
                    else
                    {
                        RestartInternal();
                    }
                }
                else if (Segments.All(s => s.State == DownloaderState.Completed))
                {
                    State = DownloaderState.Completed;
                    RaiseEvent(DownloadCompleted);
                }
                else if (Segments.Any(s => s.State == DownloaderState.Canceled))
                {
                    State = DownloaderState.Canceled;
                }
            });
        }

        private void RaiseEvent(EventHandler<EventArgs> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is Okay")]
        private void StartSegment(FileInfo fileInfo, Segment segment, Action<byte[], long, int> callback, CancellationToken cancellationToken)
        {
            segment.State = DownloaderState.Loading;

            try
            {
                var buffer = new byte[BufferSize];

                var position = segment.Start + segment.BytesRead;

                using (var stream = HttpWebRequestHelper.CreateStream(fileInfo.Location, position, segment.End))
                {
                    while (position < segment.End)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var count = (int)Math.Min(buffer.Length, segment.End - position);

                        var length = stream.Read(buffer, 0, count);

                        callback(buffer, position, length);

                        segment.BytesRead += length;
                        position += length;
                    }
                }

                segment.State = DownloaderState.Completed;
            }
            catch (OperationCanceledException)
            {
                segment.State = DownloaderState.Canceled;
            }
            catch (Exception)
            {
                segment.State = DownloaderState.Failed;
            }
        }

        private void DisposeCancellationTokenSource()
        {
            if (TokenSource == null)
            {
                return;
            }

            TokenSource.Cancel();

            if (SegmentTasks != null)
            {
                Task.WaitAll(SegmentTasks);

                foreach (var segmentTask in SegmentTasks)
                {
                    segmentTask.Dispose();
                }

                SegmentTasks = null;
            }

            TokenSource.Dispose();
            TokenSource = null;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing && _disposed)
            {
                return;
            }

            lock (_lock)
            {
                DisposeCancellationTokenSource();
                ChunkCallback = null;
                _disposed = true;
            }
        }
    }
}
