﻿using System;
using System.IO;
using System.Net;

namespace WavePlayer.Network
{
    internal static class HttpWebRequestHelper
    {
        static HttpWebRequestHelper()
        {
            ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;
            DefaultTimeout = TimeSpan.FromSeconds(5);
        }

        public static TimeSpan DefaultTimeout { get; set; }

        public static FileInfo GetRemoteFileInfo(Uri location)
        {
            var request = CreateRequest(location);

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
            var request = CreateRequest(location);

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

        private static HttpWebRequest CreateRequest(Uri location)
        {
            var request = (HttpWebRequest)WebRequest.Create(location);

            request.Timeout = (int)DefaultTimeout.TotalMilliseconds;

            return request;
        }
    }
}