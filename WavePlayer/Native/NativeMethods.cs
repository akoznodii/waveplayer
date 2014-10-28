using System;
using System.Runtime.InteropServices;

namespace WavePlayer.Native
{
    internal static class NativeMethods
    {
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int ERROR_NO_MORE_ITEMS = 259;

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstUrlCacheEntry([In]string searchPattern, [Out] IntPtr buffer, [Out, In] ref int bufferSize);

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindCloseUrlCache([In] IntPtr enumHandle);

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextUrlCacheEntry([In] IntPtr enumHandle, [Out] IntPtr buffer, [Out, In] ref int bufferSize);

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteUrlCacheEntry([In] string urlName);
    }
}
