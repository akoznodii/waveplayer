using System;
using System.Runtime.InteropServices;

namespace WavePlayer.Native.Types
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct InternetCacheEntryInfo
    {
        internal uint StructSize;
        internal string SourceUrlName;
        internal string LocalFileName;
        internal uint CacheEntryType;
        internal uint UseCount;
        internal uint HitRate;
        internal uint SizeLow;
        internal uint SizeHigh;
        internal FileTime LastModifiedTime;
        internal FileTime ExpireTime;
        internal FileTime LastAccessTime;
        internal FileTime LastSyncTime;
        internal IntPtr HeaderInfo;
        internal uint HeaderInfoSize;
        internal string FileExtension;
        internal uint ExemptDelta;
    }
}
