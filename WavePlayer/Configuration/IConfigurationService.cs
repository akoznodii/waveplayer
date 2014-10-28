using VK.OAuth;

namespace WavePlayer.Configuration
{
    public interface IConfigurationService
    {
        long ApplicationId { get; }

        AccessToken AccessToken { get; set; }

        AccessRights AccessRights { get; }

        int CultureId { get; set; }

        string Theme { get; set; }

        string AccentColor { get; set; }
    }
}
