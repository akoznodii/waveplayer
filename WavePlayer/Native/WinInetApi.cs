using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using WavePlayer.Native.Types;

namespace WavePlayer.Native
{
    public static class WinInetApi
    {
        private const string CookieSearchPattern = "cookie:";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Parameter name is appropriate")]
        public static ICollection<string> FindUrlCookieEntries(string urlPattern)
        {
            if (string.IsNullOrEmpty(urlPattern))
            {
                throw new ArgumentNullException("urlPattern");
            }

            return FindUrlCacheEntries(urlPattern, CookieSearchPattern);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Parameter name is appropriate")]
        public static void DeleteUrlCacheEntry(string urlName)
        {
            if (string.IsNullOrEmpty(urlName))
            {
                throw new ArgumentNullException("urlName");
            }

            if (NativeMethods.DeleteUrlCacheEntry(urlName))
            {
                ThrowIfLastError();
            }
        }

        private static ICollection<string> FindUrlCacheEntries(string urlPattern, string searchPattern)
        {
            var result = new List<string>();

            var bufferSize = 0;
            var buffer = IntPtr.Zero;
            var enumHandle = IntPtr.Zero;

            enumHandle = NativeMethods.FindFirstUrlCacheEntry(searchPattern, buffer, ref bufferSize);

            if (enumHandle == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();

                if (error == NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                {
                    buffer = Marshal.AllocHGlobal(bufferSize);
                }
                else if (error == NativeMethods.ERROR_NO_MORE_ITEMS)
                {
                    return result;
                }
                else
                {
                    throw new Win32Exception(error);
                }
            }

            try
            {
                enumHandle = NativeMethods.FindFirstUrlCacheEntry(searchPattern, buffer, ref bufferSize);

                if (enumHandle == IntPtr.Zero)
                {
                    ThrowIfLastError();
                }

                var firstEntry = (InternetCacheEntryInfo)Marshal.PtrToStructure(buffer, typeof(InternetCacheEntryInfo));

                if (Regex.IsMatch(firstEntry.SourceUrlName, urlPattern, RegexOptions.IgnoreCase))
                {
                    result.Add(firstEntry.SourceUrlName);
                }

                while (true)
                {
                    if (!NativeMethods.FindNextUrlCacheEntry(enumHandle, buffer, ref bufferSize))
                    {
                        var error = Marshal.GetLastWin32Error();

                        if (error == NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                        {
                            if (buffer != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(buffer);
                                buffer = IntPtr.Zero;
                            }

                            buffer = Marshal.AllocHGlobal(bufferSize);
                            continue;
                        }

                        if (error == NativeMethods.ERROR_NO_MORE_ITEMS)
                        {
                            break;
                        }
                        else
                        {
                            throw new Win32Exception(error);
                        }
                    }

                    var nextEntry = (InternetCacheEntryInfo)Marshal.PtrToStructure(buffer, typeof(InternetCacheEntryInfo));

                    if (Regex.IsMatch(nextEntry.SourceUrlName, urlPattern, RegexOptions.IgnoreCase))
                    {
                        result.Add(nextEntry.SourceUrlName);
                    }
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }

                if (enumHandle != IntPtr.Zero &&
                    !NativeMethods.FindCloseUrlCache(enumHandle))
                {
                    ThrowIfLastError();
                }
            }

            return result;
        }

        private static void ThrowIfLastError()
        {
            var error = Marshal.GetLastWin32Error();

            if (error != NativeMethods.ERROR_SUCCESS)
            {
                throw new Win32Exception(error);
            }
        }
    }
}
