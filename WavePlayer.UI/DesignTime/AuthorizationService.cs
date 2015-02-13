using System;
using VK.OAuth;
using WavePlayer.Authorization;
using WavePlayer.Users;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly User _user = new User()
        {
            Id = 1,
            FirstName = "Luke",
            LastName = "Skywalker",
            Photo = new Uri(@"http://cs620226.vk.me/v620226484/e869/TIQ21EfKvt0.jpg")
        };

        public Uri LoginUri
        {
            get { return new Uri(@"http://localhost:8999/login");}
        }

        public Uri LogoutUri
        {
            get { return new Uri(@"http://localhost:8999/logout"); }
        }

        public Uri SignupUri
        {
            get { return new Uri(@"http://localhost:8999/singin"); }
        }

        public User CurrentUser
        {
            get { return _user; }
        }

        public bool CanRetrieveToken(Uri uri)
        {
            return true;
        }

        public void RetrieveAccessToken(Uri redirectedUri)
        {
        }

        public void SetAccessToken(AccessToken accessToken)
        {
        }

        public void LoadUserInfo()
        {
        }

        public void Logout()
        {
        }
        
        public void TrackUser()
        {
        }
    }
}

#endif
