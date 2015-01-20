namespace WavePlayer.Network
{
    internal class Segment
    {
        public long Start { get; set; }

        public long End { get; set; }

        public long BytesRead { get; set; }

        public DownloaderState State { get; set; }
    }
}
