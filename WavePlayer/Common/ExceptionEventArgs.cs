using System;

namespace WavePlayer.Common
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception ErrorOccurred { get; set; }
    }
}
