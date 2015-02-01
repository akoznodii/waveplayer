using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WavePlayer.Fmod.Native
{
    internal static class NativeMethods
    {
        #region System API functions

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_Create"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode Create(ref IntPtr system);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_Release"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode ReleaseSystem(IntPtr system);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_Init"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode Init(IntPtr system, int maxChannels, InitFlags flags, IntPtr extraDriverData);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1", Justification = "It is OK")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_CreateSound"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode CreateSound(IntPtr system, string path, SoundMode mode, IntPtr extraInfo, ref IntPtr sound);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_PlaySound"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, [MarshalAs(UnmanagedType.Bool)] bool paused, ref IntPtr channel);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_Update"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode Update(IntPtr system);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_GetVersion"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetVersion(IntPtr system, out uint version);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_GetMasterChannelGroup"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetMasterChannelGroup(IntPtr system, ref IntPtr channelGroup);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_SetFileSystem"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetFileSystem(IntPtr system, OpenCallback openCallback, CloseCallback closeCallback, ReadCallback readCallback, SeekCallback seekCallback, AsyncReadCallback AsyncReadCallback, AsyncCancelCallback cancelCallback, int blockAlign);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_SetStreamBufferSize"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetStreamBufferSize(IntPtr system, uint bufferSize, TimeUnit timeUnit);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_System_CreateDSPByType"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode CreateDsp(IntPtr system, DspType dspType, ref IntPtr dsp);

        #endregion

        #region Sound API functions

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Sound_Release"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode ReleaseSound(IntPtr sound);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Sound_GetLength"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetLength(IntPtr sound, out uint length, TimeUnit timeUnit);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Sound_GetOpenState"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetOpenState(IntPtr sound, out OpenState openState, out uint percentBuffered, [MarshalAs(UnmanagedType.Bool)] out bool starving, [MarshalAs(UnmanagedType.Bool)] out bool diskBusy);

        #endregion

        #region Channel API functions

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_Stop"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode Stop(IntPtr channel);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetMute"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetMute(IntPtr channel, [MarshalAs(UnmanagedType.Bool)] bool mute);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_GetMute"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetMute(IntPtr channel, [MarshalAs(UnmanagedType.Bool)] out bool mute);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetVolume"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetVolume(IntPtr channel, float volume);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_GetVolume"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetVolume(IntPtr channel, out float volume);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetPaused"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetPaused(IntPtr channel, [MarshalAs(UnmanagedType.Bool)] bool paused);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_GetPaused"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetPaused(IntPtr channel, [MarshalAs(UnmanagedType.Bool)] out bool paused);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_IsPlaying"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode IsPlaying(IntPtr channel, [MarshalAs(UnmanagedType.Bool)] out bool isPlaying);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetPosition"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetPosition(IntPtr channel, uint position, TimeUnit unit);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_GetPosition"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetPosition(IntPtr channel, out uint position, TimeUnit unit);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetCallback"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetCallback(IntPtr channel, ChannelControlCallback callback);

        #endregion

        #region ChannelGroup API function

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_ChannelGroup_Release"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode ReleaseChannelGroup(IntPtr channelGroup);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetMute"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetMuteChannelGroup(IntPtr channelGroup, [MarshalAs(UnmanagedType.Bool)] bool mute);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_ChannelGroup_GetMute"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetMuteChannelGroup(IntPtr channelGroup, [MarshalAs(UnmanagedType.Bool)] out bool mute);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_SetVolume"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetVolumeChannelGroup(IntPtr channelGroup, float volume);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_Channel_GetVolume"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetVolumeChannelGroup(IntPtr channelGroup, out float volume);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_ChannelGroup_GetNumDSPs"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetDspNumber(IntPtr channelGroup, out int number);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_ChannelGroup_AddDSP"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode AddDsp(IntPtr channelGroup, int index, IntPtr dsp);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_ChannelGroup_RemoveDSP"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode RemoveDsp(IntPtr channelGroup, IntPtr dsp);

        #endregion

        #region DSP API functions

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_DSP_Release"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode ReleaseDsp(IntPtr dsp);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_DSP_SetActive"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetActive(IntPtr dsp, [MarshalAs(UnmanagedType.Bool)]bool active);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_DSP_GetActive"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetActive(IntPtr dsp, [MarshalAs(UnmanagedType.Bool)] out bool active);

        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_DSP_SetParameterFloat"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode SetParameter(IntPtr dsp, int index, float value);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "3", Justification = "It is OK")]
        [DllImport(FmodLibrary.Name, EntryPoint = "FMOD_DSP_GetParameterFloat"), SuppressUnmanagedCodeSecurity]
        public static extern ErrorCode GetParameter(IntPtr dsp, int index, out float value1, string value2, int length);
        
        #endregion
    }
}
