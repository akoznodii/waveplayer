using System;
using System.Runtime.InteropServices;

namespace WavePlayer.Native.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FileTime
    {
        public uint LowDateTime;
        public uint HighDateTime;
    }
}
