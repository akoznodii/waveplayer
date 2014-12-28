using System.Globalization;

namespace WavePlayer.Requests
{
    internal static class RequestHelper
    {
        public static string CreateKey(this RequestBase request)
        {
            var albumAudiosRequest = request as AlbumAudiosRequest;

            if (albumAudiosRequest != null)
            {
                return CreateAlbumAudiosRequestKey(albumAudiosRequest.AlbumId, albumAudiosRequest.OwnerId, albumAudiosRequest.OwnerIsGroup); 
            }

            var ownerAlbumsRequest = request as OwnerAlbumsRequest;

            if (ownerAlbumsRequest != null)
            {
                return CreateOwnerAlbumsRequestKey(ownerAlbumsRequest.OwnerId, ownerAlbumsRequest.OwnerIsGroup);
            }

            var popularAudiosRequest = request as PopularAudiosRequest;

            if (popularAudiosRequest != null)
            {
                return CreatePopularAudiosRequestKey(popularAudiosRequest.GenreId, popularAudiosRequest.OnlyForeign);
            }

            var recommendedAudiosRequest = request as RecommendedAudiosRequest;

            if (recommendedAudiosRequest != null)
            {
                return CreateRecommendedAudiosRequestKey(recommendedAudiosRequest.UserId, recommendedAudiosRequest.Shuffle);
            }

            var searchAudiosRequest = request as SearchAudiosRequest;

            if (searchAudiosRequest != null)
            {
                return CreateSearchAudiosRequestKey(searchAudiosRequest.Query);
            }

            var userFriendsRequest = request as UserFriendsRequest;

            if (userFriendsRequest != null)
            {
                return CreateUserFriendsRequestKey(userFriendsRequest.UserId);
            }

            var userGroupsRequest = request as UserGroupsRequest;

            if (userGroupsRequest != null)
            {
                return CreateUserGroupsRequestKey(userGroupsRequest.UserId);
            }

            return null;
        }

        public static string CreateAlbumAudiosRequestKey(long albumId, long ownerId, bool ownerIsGroup)
        {
            return string.Format(CultureInfo.InvariantCulture, "AlbumAudiosRequest AlbumId:{0}; OwnerId:{1}; OwnerIsGroup:{2}", albumId, ownerId, ownerIsGroup);
        }

        public static string CreateOwnerAlbumsRequestKey(long ownerId, bool ownerIsGroup)
        {
            return string.Format(CultureInfo.InvariantCulture, "OwnerAlbumsRequest OwnerId:{0}; OwnerIsGroup:{1}", ownerId, ownerIsGroup);
        }

        public static string CreatePopularAudiosRequestKey(long genreId, bool onlyForeign)
        {
            return string.Format(CultureInfo.InvariantCulture, "PopularAudiosRequest GenreId:{0}; OnlyForeign:{1}", genreId, onlyForeign);
        }

        public static string CreateRecommendedAudiosRequestKey(long userId, bool shuffle)
        {
            return string.Format(CultureInfo.InvariantCulture, "PopularAudiosRequest UserId:{0}; Shuffle:{1}", userId, shuffle);
        }

        public static string CreateUserGroupsRequestKey(long userId)
        {
            return string.Format(CultureInfo.InvariantCulture, "UserGroupsRequest UserId:{0}", userId);
        }

        public static string CreateUserFriendsRequestKey(long userId)
        {
            return string.Format(CultureInfo.InvariantCulture, "UserFriendsRequest UserId:{0}", userId);
        }

        public static string CreateSearchAudiosRequestKey(string query)
        {
            return string.Format(CultureInfo.InvariantCulture, "SearchAudiosRequest Query:{0}", query);
        }
    }
}
