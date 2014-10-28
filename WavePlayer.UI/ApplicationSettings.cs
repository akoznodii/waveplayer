using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using MahApps.Metro;
using VK.OAuth;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI
{
    public static class ApplicationSettings
    {
        public static long ApplicationId
        {
            get
            {
                return 3329681;
            }
        }

        public static int LanguageId
        {
            get
            {
                var configuration = Settings.Default;

                return configuration.LanguageId;
            }

            set
            {
                var configuration = Settings.Default;

                configuration.LanguageId = value;

                Settings.Default.Save();
            }
        }

        public static AccessRights AccessRights
        {
            get
            {
                var configuration = Settings.Default;

                return configuration.AccessRights;
            }
        }

        public static string Theme
        {
            get
            {
                var configuration = Settings.Default;

                return configuration.Theme;
            }

            set
            {
                var configuration = Settings.Default;

                configuration.Theme = value;

                Settings.Default.Save();
            }
        }

        public static string AccentColor
        {
            get
            {
                var configuration = Settings.Default;

                return configuration.AccentColor;
            }

            set
            {
                var configuration = Settings.Default;

                configuration.AccentColor = value;

                Settings.Default.Save();
            }
        }

        public static AccessToken AccessToken
        {
            get
            {
                var configuration = Settings.Default;

                if (string.IsNullOrEmpty(configuration.AccessToken))
                {
                    return null;
                }

                var encryptedData = Convert.FromBase64String(configuration.AccessToken);
                var data = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);

                using (var stream = new MemoryStream(data))
                {
                    var binaryForramter = new BinaryFormatter();
                    return (AccessToken)binaryForramter.Deserialize(stream);
                }
            }

            set
            {
                var configuration = Settings.Default;

                var token = string.Empty;

                if (value != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        var binaryForramter = new BinaryFormatter();
                        binaryForramter.Serialize(stream, value);

                        var encryptedData = ProtectedData.Protect(stream.ToArray(), null, DataProtectionScope.CurrentUser);
                        token = Convert.ToBase64String(encryptedData);
                    }
                }

                configuration.AccessToken = token;
                configuration.Save();
            }
        }
    }
}
