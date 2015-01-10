using WavePlayer.Authorization;
using WavePlayer.Media;
using WavePlayer.Providers;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal static class DesignData
    {
        private static readonly IVkDataProvider _vkDataProvider = new VkDataProvider();
        private static readonly IAuthorizationService _authorizationService = new AuthorizationService();      
        private static readonly IPlayerEngine _playerEngine = new PlayerEngine();
        private static readonly IPlayer _player = new Player(_playerEngine);

        public static IVkDataProvider VkDataProvider { get { return _vkDataProvider; } }

        public static IAuthorizationService AuthorizationService { get { return _authorizationService; } }

        public static IPlayer Player { get { return _player; } }

        public static IPlayerEngine PlayerEngine { get { return _playerEngine; } }
    }
}

#endif
