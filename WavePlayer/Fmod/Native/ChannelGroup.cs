using System;

namespace WavePlayer.Fmod.Native
{
    internal class ChannelGroup : Handle
    {
        internal ChannelGroup(IntPtr soundHandle)
        {
            SetHandle(soundHandle);
        }

        public float Volume
        {
            get
            {
                var result = 0.0f;
                ErrorHandler.ThrowIfError(NativeMethods.GetVolumeChannelGroup(DangerousGetHandle(), out result));
                return result;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetVolumeChannelGroup(DangerousGetHandle(), value));
            }
        }

        public bool Mute
        {
            get
            {
                var value = false;
                ErrorHandler.ThrowIfError(NativeMethods.GetMuteChannelGroup(DangerousGetHandle(), out value));
                return value;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetMuteChannelGroup(DangerousGetHandle(), value));
            }
        }

        public void Add(Dsp dsp)
        {
            var index = 0;

            ErrorHandler.ThrowIfError(NativeMethods.GetDspNumber(DangerousGetHandle(), out index));

            ErrorHandler.ThrowIfError(NativeMethods.AddDsp(DangerousGetHandle(), index, dsp.DangerousGetHandle()));
        }

        public void Remove(Dsp dsp)
        {
            ErrorHandler.ThrowIfError(NativeMethods.RemoveDsp(DangerousGetHandle(), dsp.DangerousGetHandle()));
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
            {
                return true;
            }

            NativeMethods.ReleaseChannelGroup(handle);
            SetHandleAsInvalid();
            return true;
        }
    }
}
