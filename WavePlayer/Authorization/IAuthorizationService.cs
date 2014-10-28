using System;
using VK.OAuth;
using WavePlayer.Users;

namespace WavePlayer.Authorization
{
    public interface IAuthorizationService
    {
        Uri LoginUri { get; }

        Uri LogoutUri { get; }

        Uri SignupUri { get; }

        User CurrentUser { get; }

        bool CanRetrieveToken(Uri uri);

        void RetrieveAccessToken(Uri redirectedUri);

        void SetAccessToken(AccessToken accessToken);

        void LoadUserInfo();

        void Logout();
    }
}
