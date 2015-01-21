using System;
using System.IO;
using System.Net;

namespace WavePlayer.Network
{
    internal static class HttpWebRequestHelper
    {
        private const int DefaultTimeout = 30000;
        
        public static FileInfo GetRemoteFileInfo(Uri location)
        {
            var request = (HttpWebRequest)WebRequest.Create(location);

            var response = request.GetResponse();

            return new FileInfo()
            {
                Location = location,
                ContentLength = response.ContentLength,
                AcceptRanges = String.Compare(response.Headers["Accept-Ranges"], "bytes", StringComparison.OrdinalIgnoreCase) == 0
            };
        }

        public static Stream CreateStream(Uri location, long startPosition, long endPosition)
        {
            var request = (HttpWebRequest)WebRequest.Create(location);

            request.Timeout = DefaultTimeout;

            if (endPosition != 0)
            {
                request.AddRange(startPosition, endPosition);
            }
            else
            {
                request.AddRange(startPosition);
            }

            var response = request.GetResponse();

            return response.GetResponseStream();
        }
    }
}
