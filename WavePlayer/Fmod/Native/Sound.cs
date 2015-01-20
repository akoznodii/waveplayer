using System;

namespace WavePlayer.Fmod.Native
{
    internal class Sound : Handle
    {
        internal Sound(IntPtr soundHandle)
        {
            SetHandle(soundHandle);
            TimeUnit = TimeUnit.FMOD_TIMEUNIT_MS;
        }

        public TimeUnit TimeUnit { get; set; }

        public uint Length
        {
            get
            {
                var length = 0u;
                ErrorHandler.ThrowIfError(NativeMethods.GetLength(DangerousGetHandle(), out length, TimeUnit));
                return length;
            }
        }

        public SoundState State
        {
            get
            {
                var openState = OpenState.FMOD_OPENSTATE_READY;
                var percentBuffered = 0u;
                var starving = false;
                var diskBusy = false;

                ErrorHandler.ThrowIfError(NativeMethods.GetOpenState(DangerousGetHandle(), out openState, out percentBuffered, out starving, out diskBusy));

                return new SoundState(openState, percentBuffered, starving, diskBusy);
            }
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
            {
                return true;
            }

            NativeMethods.ReleaseSound(handle);
            SetHandleAsInvalid();
            return true;
        }
    }
}
