using System;
using System.Collections.Generic;
using WavePlayer.Audios;
using WavePlayer.Groups;
using WavePlayer.Localization;
using WavePlayer.Users;

namespace WavePlayer.Providers
{
    public interface IVkDataProvider : ILocalizable
    {
        event EventHandler<AudioEventArgs> AudioAdded;

        event EventHandler<AudioEventArgs> AudioRemoved;

        ICollection<Genre> GetGenres();

        ICollection<User> GetUserFriends(User user);

        ICollection<Group> GetUserGroups(User user);

        ICollection<Album> GetUserAlbums(User user);

        ICollection<Album> GetGroupAlbums(Group group);

        ICollection<Audio> GetAlbumAudios(Album album);

        ICollection<Audio> GetPopularAudios(Genre genre, bool onlyForeign);

        ICollection<Audio> GetRecommendedAudios(User user, bool shuffle);

        ICollection<Audio> GetSearchAudios(string query);

        Lyrics GetLyrics(Audio audio);

        bool Remove(Audio audio);

        Audio Add(Audio audio);

        void LoadCollection<T>(ICollection<T> collection);

        void Clear();
    }
}