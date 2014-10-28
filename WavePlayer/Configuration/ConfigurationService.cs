using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using VK.OAuth;

namespace WavePlayer.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        public long ApplicationId
        {
            get
            {
                return 3329681;
            }
        }

        public int CultureId
        {
            get
            {
                var configuration = AppSettings.Default;

                return configuration.CultureId;
            }

            set
            {
                var configuration = AppSettings.Default;

                configuration.CultureId = value;

                AppSettings.Default.Save();
            }
        }

        public AccessRights AccessRights
        {
            get
            {
                var configuration = AppSettings.Default;

                return configuration.AccessRights;
            }
        }

        public string Theme
        {
            get
            {
                var configuration = AppSettings.Default;

                return configuration.Theme;
            }

            set
            {
                var configuration = AppSettings.Default;

                configuration.Theme = value;

                AppSettings.Default.Save();
            }
        }

        public string AccentColor
        {
            get
            {
                var configuration = AppSettings.Default;

                return configuration.AccentColor;
            }

            set
            {
                var configuration = AppSettings.Default;

                configuration.AccentColor = value;

                AppSettings.Default.Save();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Getter should be save")]
        public AccessToken AccessToken
        {
            get
            {
                var configuration = AppSettings.Default;

                if (string.IsNullOrEmpty(configuration.AccessToken))
                {
                    return null;
                }

                try
                {
                    var encryptedData = Convert.FromBase64String(configuration.AccessToken);
                    var data = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);

                    using (var stream = new MemoryStream(data))
                    {
                        var binaryForramter = new BinaryFormatter();
                        return (AccessToken)binaryForramter.Deserialize(stream);
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("Failed to desirialize access token: {0}", exception);

                    return null;
                }
            }

            set
            {
                var configuration = AppSettings.Default;

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
