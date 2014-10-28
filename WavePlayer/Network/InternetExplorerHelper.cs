using System.Diagnostics;
using System.Globalization;
using System.Linq;
using WavePlayer.Native;

namespace WavePlayer.Network
{
    public static class InternetExplorerHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Parameter name is appropriate")]
        public static void DeleteCookies(string urlPattern)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Deleting internet explorer cookies for url address: {0}", urlPattern));

            var cookieEntries = WinInetApi.FindUrlCookieEntries(urlPattern);

            if (!cookieEntries.Any())
            {
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Internet explorer doesn't have any cookies for url address: {0}", urlPattern));
                return;
            }

            foreach (var cookieEntry in cookieEntries)
            {
                WinInetApi.DeleteUrlCacheEntry(cookieEntry);

                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Cookie entry deleted: {0}", cookieEntry));
            }
        }
    }
}
