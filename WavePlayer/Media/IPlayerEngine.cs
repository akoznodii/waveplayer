using System;
using System.Diagnostics.CodeAnalysis;
using WavePlayer.Common;

namespace WavePlayer.Media
{
    public interface IPlayerEngine
    {
        event EventHandler<ExceptionEventArgs> MediaFailed;

        event EventHandler MediaOpened;
        
        event EventHandler MediaEnded;
        
        event EventHandler BufferingStarted;
        
        event EventHandler BufferingEnded;

        event EventHandler PlaybackStateChanged;

        PlaybackState PlaybackState { get; }
        
        Uri Source { get; }

        double Volume { get; set; }

        bool IsMuted { get; set; }

        TimeSpan Position { get; set; }

        TimeSpan Duration { get; }

        void Open(Uri source);

        void Play();

        void Pause();

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Solution is developing in C#")]
        void Stop();
        
        void Close();
    }
}
