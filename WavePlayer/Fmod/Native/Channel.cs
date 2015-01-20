using System;

namespace WavePlayer.Fmod.Native
{
    internal class Channel : Handle
    {
        internal Channel(IntPtr soundHandle)
        {
            SetHandle(soundHandle);
            TimeUnit = TimeUnit.FMOD_TIMEUNIT_MS;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public bool IsPlayingNow
        {
            get
            {
                var result = false;
                ErrorHandler.ThrowIfError(NativeMethods.IsPlaying(DangerousGetHandle(), out result));
                return result;
            }
        }

        public bool Paused
        {
            get
            {
                var result = false;
                ErrorHandler.ThrowIfError(NativeMethods.GetPaused(DangerousGetHandle(), out result));
                return result;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetPaused(DangerousGetHandle(), value));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public float Volume
        {
            get
            {
                var result = 0.0f;
                ErrorHandler.ThrowIfError(NativeMethods.GetVolume(DangerousGetHandle(), out result));
                return result;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetVolume(DangerousGetHandle(), value));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public bool Mute
        {
            get
            {
                var value = false;
                ErrorHandler.ThrowIfError(NativeMethods.GetMute(DangerousGetHandle(), out value));
                return value;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetMute(DangerousGetHandle(), value));
            }
        }

        public TimeUnit TimeUnit { get; set; }

        public uint Position
        {
            get
            {
                var result = 0u;
                ErrorHandler.ThrowIfError(NativeMethods.GetPosition(DangerousGetHandle(), out result, TimeUnit));
                return result;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetPosition(DangerousGetHandle(), value, TimeUnit));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public void Stop()
        {
            ErrorHandler.ThrowIfError(NativeMethods.Stop(DangerousGetHandle()));
        }

        public void SetCallback(ChannelControlCallback callback)
        {
            ErrorHandler.ThrowIfError(NativeMethods.SetCallback(DangerousGetHandle(), callback));
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
            {
                return true;
            }

            NativeMethods.Stop(handle);
            SetHandleAsInvalid();
            return true;
        }
    }
}
