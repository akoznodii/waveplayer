using System;

namespace WavePlayer.Network
{
    internal class FileInfo
    {
        public long ContentLength { get; set; }

        public bool AcceptRanges { get; set; }

        public Uri Location { get; set; }
    }
}
