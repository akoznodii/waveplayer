using System;
using System.Globalization;

namespace WavePlayer.Fmod.Native
{
    internal static class ErrorHandler
    {
        public static void ThrowIfError(ErrorCode errorCode)
        {
            if (errorCode == ErrorCode.FMOD_OK)
            {
                return;
            }

            var message = string.Format(CultureInfo.InvariantCulture, "FMOD API function call has failed with error code: {0}", errorCode);
            throw new InvalidOperationException(message);
        }
    }

    internal enum ErrorCode : int
    {
        FMOD_OK,
        FMOD_ERR_BADCOMMAND,
        FMOD_ERR_CHANNEL_ALLOC,
        FMOD_ERR_CHANNEL_STOLEN,
        FMOD_ERR_DMA,
        FMOD_ERR_DSP_CONNECTION,
        FMOD_ERR_DSP_DONTPROCESS,
        FMOD_ERR_DSP_FORMAT,
        FMOD_ERR_DSP_INUSE,
        FMOD_ERR_DSP_NOTFOUND,
        FMOD_ERR_DSP_RESERVED,
        FMOD_ERR_DSP_SILENCE,
        FMOD_ERR_DSP_TYPE,
        FMOD_ERR_FILE_BAD,
        FMOD_ERR_FILE_COULDNOTSEEK,
        FMOD_ERR_FILE_DISKEJECTED,
        FMOD_ERR_FILE_EOF,
        FMOD_ERR_FILE_ENDOFDATA,
        FMOD_ERR_FILE_NOTFOUND,
        FMOD_ERR_FORMAT,
        FMOD_ERR_HEADER_MISMATCH,
        FMOD_ERR_HTTP,
        FMOD_ERR_HTTP_ACCESS,
        FMOD_ERR_HTTP_PROXY_AUTH,
        FMOD_ERR_HTTP_SERVER_ERROR,
        FMOD_ERR_HTTP_TIMEOUT,
        FMOD_ERR_INITIALIZATION,
        FMOD_ERR_INITIALIZED,
        FMOD_ERR_INTERNAL,
        FMOD_ERR_INVALID_FLOAT,
        FMOD_ERR_INVALID_HANDLE,
        FMOD_ERR_INVALID_PARAM,
        FMOD_ERR_INVALID_POSITION,
        FMOD_ERR_INVALID_SPEAKER,
        FMOD_ERR_INVALID_SYNCPOINT,
        FMOD_ERR_INVALID_THREAD,
        FMOD_ERR_INVALID_VECTOR,
        FMOD_ERR_MAXAUDIBLE,
        FMOD_ERR_MEMORY,
        FMOD_ERR_MEMORY_CANTPOINT,
        FMOD_ERR_NEEDS3D,
        FMOD_ERR_NEEDSHARDWARE,
        FMOD_ERR_NET_CONNECT,
        FMOD_ERR_NET_SOCKET_ERROR,
        FMOD_ERR_NET_URL,
        FMOD_ERR_NET_WOULD_BLOCK,
        FMOD_ERR_NOTREADY,
        FMOD_ERR_OUTPUT_ALLOCATED,
        FMOD_ERR_OUTPUT_CREATEBUFFER,
        FMOD_ERR_OUTPUT_DRIVERCALL,
        FMOD_ERR_OUTPUT_FORMAT,
        FMOD_ERR_OUTPUT_INIT,
        FMOD_ERR_OUTPUT_NODRIVERS,
        FMOD_ERR_PLUGIN,
        FMOD_ERR_PLUGIN_MISSING,
        FMOD_ERR_PLUGIN_RESOURCE,
        FMOD_ERR_PLUGIN_VERSION,
        FMOD_ERR_RECORD,
        FMOD_ERR_REVERB_CHANNELGROUP,
        FMOD_ERR_REVERB_INSTANCE,
        FMOD_ERR_SUBSOUNDS,
        FMOD_ERR_SUBSOUND_ALLOCATED,
        FMOD_ERR_SUBSOUND_CANTMOVE,
        FMOD_ERR_TAGNOTFOUND,
        FMOD_ERR_TOOMANYCHANNELS,
        FMOD_ERR_TRUNCATED,
        FMOD_ERR_UNIMPLEMENTED,
        FMOD_ERR_UNINITIALIZED,
        FMOD_ERR_UNSUPPORTED,
        FMOD_ERR_VERSION,
        FMOD_ERR_EVENT_ALREADY_LOADED,
        FMOD_ERR_EVENT_LIVEUPDATE_BUSY,
        FMOD_ERR_EVENT_LIVEUPDATE_MISMATCH,
        FMOD_ERR_EVENT_LIVEUPDATE_TIMEOUT,
        FMOD_ERR_EVENT_NOTFOUND,
        FMOD_ERR_STUDIO_UNINITIALIZED,
        FMOD_ERR_STUDIO_NOT_LOADED,
        FMOD_ERR_INVALID_STRING,
        FMOD_ERR_ALREADY_LOCKED,
        FMOD_ERR_NOT_LOCKED
    }

    [Flags]
    internal enum SoundMode : uint
    {
        FMOD_DEFAULT = 0x00000000,
        FMOD_LOOP_OFF = 0x00000001,
        FMOD_LOOP_NORMAL = 0x00000002,
        FMOD_LOOP_BIDI = 0x00000004,
        FMOD_2D = 0x00000008,
        FMOD_3D = 0x00000010,
        FMOD_CREATESTREAM = 0x00000080,
        FMOD_CREATESAMPLE = 0x00000100,
        FMOD_CREATECOMPRESSEDSAMPLE = 0x00000200,
        FMOD_OPENUSER = 0x00000400,
        FMOD_OPENMEMORY = 0x00000800,
        FMOD_OPENMEMORY_POINT = 0x10000000,
        FMOD_OPENRAW = 0x00001000,
        FMOD_OPENONLY = 0x00002000,
        FMOD_ACCURATETIME = 0x00004000,
        FMOD_MPEGSEARCH = 0x00008000,
        FMOD_NONBLOCKING = 0x00010000,
        FMOD_UNIQUE = 0x00020000,
        FMOD_3D_HEADRELATIVE = 0x00040000,
        FMOD_3D_WORLDRELATIVE = 0x00080000,
        FMOD_3D_INVERSEROLLOFF = 0x00100000,
        FMOD_3D_LINEARROLLOFF = 0x00200000,
        FMOD_3D_LINEARSQUAREROLLOFF = 0x00400000,
        FMOD_3D_INVERSETAPEREDROLLOFF = 0x00800000,
        FMOD_3D_CUSTOMROLLOFF = 0x04000000,
        FMOD_3D_IGNOREGEOMETRY = 0x40000000,
        FMOD_IGNORETAGS = 0x02000000,
        FMOD_LOWMEM = 0x08000000,
        FMOD_LOADSECONDARYRAM = 0x20000000,
        FMOD_VIRTUAL_PLAYFROMSTART = 0x80000000
    }

    [Flags]
    internal enum InitFlags : uint
    {
        FMOD_INIT_NORMAL = 0x00000000,
        FMOD_INIT_STREAM_FROM_UPDATE = 0x00000001,
        FMOD_INIT_MIX_FROM_UPDATE = 0x00000002,
        FMOD_INIT_3D_RIGHTHANDED = 0x00000004,
        FMOD_INIT_CHANNEL_LOWPASS = 0x00000100,
        FMOD_INIT_CHANNEL_DISTANCEFILTER = 0x00000200,
        FMOD_INIT_PROFILE_ENABLE = 0x00010000,
        FMOD_INIT_VOL0_BECOMES_VIRTUAL = 0x00020000,
        FMOD_INIT_GEOMETRY_USECLOSEST = 0x00040000,
        FMOD_INIT_PREFER_DOLBY_DOWNMIX = 0x00080000,
        FMOD_INIT_THREAD_UNSAFE = 0x00100000,
        FMOD_INIT_PROFILE_METER_ALL = 0x00200000
    }

    [Flags]
    internal enum TimeUnit : uint
    {
        FMOD_TIMEUNIT_MS = 0x00000001,
        FMOD_TIMEUNIT_PCM = 0x00000002,
        FMOD_TIMEUNIT_PCMBYTES = 0x00000004,
        FMOD_TIMEUNIT_RAWBYTES = 0x00000008,
        FMOD_TIMEUNIT_PCMFRACTION = 0x00000010,
        FMOD_TIMEUNIT_MODORDER = 0x00000100,
        FMOD_TIMEUNIT_MODROW = 0x00000200,
        FMOD_TIMEUNIT_MODPATTERN = 0x00000400,
        FMOD_TIMEUNIT_BUFFERED = 0x10000000
    }

    internal enum ChannelType
    {
        FMOD_CHANNELCONTROL_CHANNEL,
        FMOD_CHANNELCONTROL_CHANNELGROUP
    }

    internal enum ChannelCallbackType
    {
        FMOD_CHANNELCONTROL_CALLBACK_END,
        FMOD_CHANNELCONTROL_CALLBACK_VIRTUALVOICE,
        FMOD_CHANNELCONTROL_CALLBACK_SYNCPOINT,
        FMOD_CHANNELCONTROL_CALLBACK_OCCLUSION,
        FMOD_CHANNELCONTROL_CALLBACK_MAX
    }

    internal enum OpenState
    {
        FMOD_OPENSTATE_READY,
        FMOD_OPENSTATE_LOADING,
        FMOD_OPENSTATE_ERROR,
        FMOD_OPENSTATE_CONNECTING,
        FMOD_OPENSTATE_BUFFERING,
        FMOD_OPENSTATE_SEEKING,
        FMOD_OPENSTATE_PLAYING,
        FMOD_OPENSTATE_SETPOSITION,
        FMOD_OPENSTATE_MAX
    }

    internal enum DspType
    {
        FMOD_DSP_TYPE_UNKNOWN,
        FMOD_DSP_TYPE_MIXER,
        FMOD_DSP_TYPE_OSCILLATOR,
        FMOD_DSP_TYPE_LOWPASS,
        FMOD_DSP_TYPE_ITLOWPASS,
        FMOD_DSP_TYPE_HIGHPASS,
        FMOD_DSP_TYPE_ECHO,
        FMOD_DSP_TYPE_FADER,
        FMOD_DSP_TYPE_FLANGE,
        FMOD_DSP_TYPE_DISTORTION,
        FMOD_DSP_TYPE_NORMALIZE,
        FMOD_DSP_TYPE_LIMITER,
        FMOD_DSP_TYPE_PARAMEQ,
        FMOD_DSP_TYPE_PITCHSHIFT,
        FMOD_DSP_TYPE_CHORUS,
        FMOD_DSP_TYPE_VSTPLUGIN,
        FMOD_DSP_TYPE_WINAMPPLUGIN,
        FMOD_DSP_TYPE_ITECHO,
        FMOD_DSP_TYPE_COMPRESSOR,
        FMOD_DSP_TYPE_SFXREVERB,
        FMOD_DSP_TYPE_LOWPASS_SIMPLE,
        FMOD_DSP_TYPE_DELAY,
        FMOD_DSP_TYPE_TREMOLO,
        FMOD_DSP_TYPE_LADSPAPLUGIN,
        FMOD_DSP_TYPE_SEND,
        FMOD_DSP_TYPE_RETURN,
        FMOD_DSP_TYPE_HIGHPASS_SIMPLE,
        FMOD_DSP_TYPE_PAN,
        FMOD_DSP_TYPE_THREE_EQ,
        FMOD_DSP_TYPE_FFT,
        FMOD_DSP_TYPE_LOUDNESS_METER,
        FMOD_DSP_TYPE_ENVELOPEFOLLOWER,
        FMOD_DSP_TYPE_CONVOLUTIONREVERB,
        FMOD_DSP_TYPE_MAX
    }

    internal enum EqualizerParameters
    {
        FMOD_DSP_PARAMEQ_CENTER = 0,
        FMOD_DSP_PARAMEQ_BANDWIDTH = 1,
        FMOD_DSP_PARAMEQ_GAIN = 2
    }

    internal struct AsyncReadInfo
    {
        public IntPtr Handle;
        public uint Offset;
        public uint SizeBytes;
        public int Priority;
        public IntPtr UserData;
        public IntPtr Buffer;
        public int BytesRead;
    }

    internal struct SoundState
    {
        public SoundState(OpenState openOpenState, uint percentBuffered, bool starving, bool diskBusy)
            : this()
        {
            OpenState = openOpenState;
            PercentBuffered = percentBuffered;
            IsStarving = starving;
            DiskBusy = diskBusy;
        }

        public OpenState OpenState { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public uint PercentBuffered { get; private set; }

        public bool IsStarving { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public bool DiskBusy { get; private set; }
    }

    internal delegate ErrorCode ChannelControlCallback(IntPtr channel, ChannelType channelType, ChannelCallbackType callbackType, IntPtr commandData1, IntPtr commandData2);

    internal delegate ErrorCode OpenCallback(string fileName, ref uint fileSize, ref IntPtr handle, ref IntPtr userData);

    internal delegate ErrorCode CloseCallback(IntPtr handle, IntPtr userData);

    internal delegate ErrorCode ReadCallback(IntPtr handle, IntPtr buffer, uint sizeBytes, ref uint bytesRead, IntPtr userData);

    internal delegate ErrorCode SeekCallback(IntPtr handle, int position, IntPtr userData);

    internal delegate ErrorCode AsyncReadCallback(IntPtr handle, AsyncReadInfo info, IntPtr userData);

    internal delegate ErrorCode AsyncCancelCallback(IntPtr handle, AsyncReadInfo info, IntPtr userData);
}
