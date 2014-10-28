using System;
using System.Diagnostics;
using System.Net;
using VK;
using VK.OAuth;
using VK.Users;
using WavePlayer.Configuration;
using WavePlayer.Network;
using User = WavePlayer.Users.User;

namespace WavePlayer.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IConfigurationService _configurationService;
        private readonly VkClient _vkClient;
        private Uri _loginUri;
        private Uri _logoutUri;
        private Uri _singupUri;

        public AuthorizationService(VkClient vkClient, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _vkClient = vkClient;
        }

        public Uri LoginUri
        {
            get
            {
                if (_loginUri == null)
                {
                    _loginUri = OAuthHelper.GetLoginUri(_configurationService.ApplicationId, _configurationService.AccessRights);
                }

                return _loginUri;
            }
        }

        public Uri LogoutUri
        {
            get
            {
                if (_logoutUri == null)
                {
                    _logoutUri = OAuthHelper.GetLogoutUri();
                }

                return _logoutUri;
            }
        }

        public Uri SignupUri
        {
            get
            {
                if (_singupUri == null)
                {
                    _singupUri = new Uri(VkConfiguration.SignupAddress);
                }

                return _singupUri;
            }
        }

        public User CurrentUser { get; private set; }

        public void RetrieveAccessToken(Uri redirectedUri)
        {
            var accessToken = OAuthHelper.RetrieveAccessToken(redirectedUri);

            SetAccessToken(accessToken);
        }

        public void SetAccessToken(AccessToken accessToken)
        {
            _configurationService.AccessToken = accessToken;

            _vkClient.AccessToken = accessToken;

            CurrentUser = new User() { Id = accessToken.UserId };
        }

        public bool CanRetrieveToken(Uri uri)
        {
            return OAuthHelper.CanRetrieveToken(uri);
        }

        public void Logout()
        {
            var logout = LogoutUri;

            var webRequest = WebRequest.Create(logout);

            webRequest.GetResponse();

            var host = SignupUri.Host;

            InternetExplorerHelper.DeleteCookies(host);

            _configurationService.AccessToken = null;

            _vkClient.AccessToken = null;

            CurrentUser = null;
        }

        public void LoadUserInfo()
        {
            var user = _vkClient.Users.Get(UserFields.Photo50);
            CurrentUser.FirstName = user.FirstName;
            CurrentUser.LastName = user.LastName;
            CurrentUser.Photo = new Uri(user.Photo);
        }
    }
}
