using System;
using System.Globalization;

namespace WavePlayer.Fmod.Native
{
    internal class FmodSystem : Handle
    {
        public FmodSystem()
        {
            var systemHandle = IntPtr.Zero;

            ErrorHandler.ThrowIfError(NativeMethods.Create(ref systemHandle));

            SetHandle(systemHandle);

            CheckVersion();
        }

        public void Initialize(int maxChannels, InitFlags flags)
        {
            ErrorHandler.ThrowIfError(NativeMethods.Init(DangerousGetHandle(), maxChannels, flags, IntPtr.Zero));
        }

        public void SetBufferSize(uint bufferSize)
        {
            ErrorHandler.ThrowIfError(NativeMethods.SetStreamBufferSize(DangerousGetHandle(), bufferSize, TimeUnit.FMOD_TIMEUNIT_RAWBYTES));
        }
        
        public void Update()
        {
            ErrorHandler.ThrowIfError(NativeMethods.Update(DangerousGetHandle()));
        }

        public ChannelGroup MasterChannelGroup
        {
            get
            {
                var channelGroupHandler = IntPtr.Zero;

                ErrorHandler.ThrowIfError(NativeMethods.GetMasterChannelGroup(DangerousGetHandle(), ref channelGroupHandler));
                
                return new ChannelGroup(channelGroupHandler);
            }
        }

        public Sound CreateSound(string path, SoundMode mode)
        {
            var soundHandle = IntPtr.Zero;

            ErrorHandler.ThrowIfError(NativeMethods.CreateSound(DangerousGetHandle(), path, mode, IntPtr.Zero, ref soundHandle));

            return new Sound(soundHandle);
        }

        public Dsp CreateDsp(DspType dspType)
        {
            var dspHandle = IntPtr.Zero;

            ErrorHandler.ThrowIfError(NativeMethods.CreateDsp(DangerousGetHandle(), dspType, ref dspHandle));

            return new Dsp(dspHandle);
        }

        public Channel PlaySound(Sound sound, bool paused)
        {
            var channelHandle = IntPtr.Zero;

            ErrorHandler.ThrowIfError(NativeMethods.PlaySound(DangerousGetHandle(), sound.DangerousGetHandle(), IntPtr.Zero, paused, ref channelHandle));
            
            return new Channel(channelHandle);
        }

        public void SetFileSystem(OpenCallback openCallback, CloseCallback closeCallback, ReadCallback readCallback, SeekCallback seekCallback)
        {
            ErrorHandler.ThrowIfError(NativeMethods.SetFileSystem(DangerousGetHandle(), openCallback, closeCallback, readCallback, seekCallback, null, null, -1));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public void SetFileSystem(OpenCallback openCallback, CloseCallback closeCallback, AsyncReadCallback readCallback, AsyncCancelCallback cancelCallback)
        {
            ErrorHandler.ThrowIfError(NativeMethods.SetFileSystem(DangerousGetHandle(), openCallback, closeCallback, null, null, readCallback, cancelCallback, -1));
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
            {
                return true;
            }

            NativeMethods.ReleaseSystem(handle);
            SetHandleAsInvalid();

            return true;
        }

        private void CheckVersion()
        {
            var version = 0u;

            ErrorHandler.ThrowIfError(NativeMethods.GetVersion(DangerousGetHandle(), out version));

            if (version >= FmodLibrary.Version)
            {
                return;
            }

            NativeMethods.ReleaseSystem(handle);
            SetHandleAsInvalid();
            var message = string.Format(CultureInfo.InvariantCulture, "Library {0}.dll has version {1:X}. Minimal supported version is : {2:X}", FmodLibrary.Name, version, FmodLibrary.Version);
            throw new NotSupportedException(message);
        }
    }
}
