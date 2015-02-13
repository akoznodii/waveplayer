using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using VK;
using VK.Audios;
using VK.Collections;
using WavePlayer.Audios;
using WavePlayer.Caching;
using WavePlayer.Groups;
using WavePlayer.Requests;
using WavePlayer.Users;
using Album = WavePlayer.Audios.Album;
using Audio = WavePlayer.Audios.Audio;
using Genre = WavePlayer.Audios.Genre;
using Lyrics = WavePlayer.Audios.Lyrics;

namespace WavePlayer.Providers
{
    public sealed class VkDataProvider : IVkDataProvider, IDisposable
    {
        private const int LoadItemsCount = 50;
        private static readonly TimeSpan ItemsLifetime = TimeSpan.FromMinutes(5);

        private readonly VkClient _vkClient;
        private readonly GenericCache<RemoteCollection<Audio>> _audiosCache;
        private readonly GenericCache<RemoteCollection<Album>> _albumsCache;
        private readonly GenericCache<RemoteCollection<User>> _usersCache;
        private readonly GenericCache<RemoteCollection<Group>> _groupsCache;
        private readonly GenericCache<Lyrics> _lyricsCache;
        private readonly Lazy<ICollection<Genre>> _genres = new Lazy<ICollection<Genre>>(LoadGenres, true);

        public event EventHandler<AudioEventArgs> AudioAdded;

        public event EventHandler<AudioEventArgs> AudioRemoved;

        public VkDataProvider(VkClient vkClient)
        {
            _vkClient = vkClient;

            _audiosCache = new GenericCache<RemoteCollection<Audio>>("audios", (audio) => audio.Any(i => i.IsPlayingNow))
            {
                SlidingExpiration = ItemsLifetime
            };

            _albumsCache = new GenericCache<RemoteCollection<Album>>("albums")
            {
                SlidingExpiration = ItemsLifetime
            };

            _usersCache = new GenericCache<RemoteCollection<User>>("users")
            {
                SlidingExpiration = ItemsLifetime
            };

            _groupsCache = new GenericCache<RemoteCollection<Group>>("groups")
            {
                SlidingExpiration = ItemsLifetime
            };

            _lyricsCache = new GenericCache<Lyrics>("lyrics")
            {
                SlidingExpiration = ItemsLifetime
            };
        }

        public ICollection<Genre> GetGenres()
        {
            return _genres.Value;
        }

        public ICollection<User> GetUserFriends(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var key = RequestHelper.CreateUserFriendsRequestKey(user.Id);

            return GetCollection(_usersCache, key, () => new UserFriendsRequest { UserId = user.Id });
        }

        public ICollection<Group> GetUserGroups(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var key = RequestHelper.CreateUserGroupsRequestKey(user.Id);

            return GetCollection(_groupsCache, key, () => new UserGroupsRequest { UserId = user.Id });
        }

        public ICollection<Album> GetUserAlbums(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var key = RequestHelper.CreateOwnerAlbumsRequestKey(user.Id, false);

            return GetCollection(_albumsCache, key, () => new OwnerAlbumsRequest() { OwnerId = user.Id, OwnerIsGroup = false });
        }

        public ICollection<Album> GetGroupAlbums(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            var key = RequestHelper.CreateOwnerAlbumsRequestKey(group.Id, true);

            return GetCollection(_albumsCache, key, () => new OwnerAlbumsRequest() { OwnerId = group.Id, OwnerIsGroup = true });
        }

        public ICollection<Audio> GetAlbumAudios(Album album)
        {
            if (album == null)
            {
                throw new ArgumentNullException("album");
            }

            var key = RequestHelper.CreateAlbumAudiosRequestKey(album.Id, album.OwnerId, album.OwnerIsGroup);

            return GetCollection(_audiosCache, key, album.ToRequest);
        }

        public ICollection<Audio> GetPopularAudios(Genre genre, bool onlyForeign)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre");
            }

            var key = RequestHelper.CreatePopularAudiosRequestKey(genre.Id, onlyForeign);

            return GetCollection(_audiosCache, key, () => new PopularAudiosRequest() { GenreId = genre.Id, OnlyForeign = onlyForeign });
        }

        public ICollection<Audio> GetRecommendedAudios(User user, bool shuffle)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var key = RequestHelper.CreateRecommendedAudiosRequestKey(user.Id, shuffle);

            return GetCollection(_audiosCache, key, () => new RecommendedAudiosRequest() { UserId = user.Id, Shuffle = shuffle });
        }

        public ICollection<Audio> GetSearchAudios(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException("query");
            }

            var key = RequestHelper.CreateSearchAudiosRequestKey(query);

            return GetCollection(_audiosCache, key, () => new SearchAudiosRequest() { Query = query });
        }

        public Lyrics GetLyrics(Audio audio)
        {
            if (audio == null)
            {
                throw new ArgumentNullException("audio");
            }

            if (audio.LyricsId == 0)
            {
                throw new InvalidOperationException("Audio does not have lyrics");
            }

            var lyricsId = audio.LyricsId;
            var title = audio.Title;
            var artist = audio.Artist;

            return _lyricsCache.GetOrAdd(lyricsId.ToString(CultureInfo.InvariantCulture), () =>
            {
                var lyrics = _vkClient.Audios.GetLyrics(audio.LyricsId);

                return new Lyrics()
                {
                    Id = lyrics.Id,
                    Text = lyrics.Text,
                    Artist = artist,
                    Title = title
                };
            });
        }

        public void LoadCollection<T>(ICollection<T> collection)
        {
            var audiosCollection = collection as RemoteCollection<Audio>;

            if (audiosCollection != null)
            {
                LoadCollection(_audiosCache, audiosCollection, LoadAudios, audio => audio.ToModel());
            }

            var albumsCollection = collection as RemoteCollection<Album>;

            if (albumsCollection != null)
            {
                PreviewLoadAlbums(albumsCollection);
                LoadCollection(_albumsCache, albumsCollection, LoadAlbums, album => album.ToModel());
            }

            var usersCollection = collection as RemoteCollection<User>;

            if (usersCollection != null)
            {
                LoadCollection(_usersCache, usersCollection, LoadUsers, user => user.ToModel());
            }

            var groupsCollection = collection as RemoteCollection<Group>;

            if (groupsCollection != null)
            {
                LoadCollection(_groupsCache, groupsCollection, LoadGroups, group => group.ToModel());
            }
        }

        public bool Remove(Audio audio)
        {
            var userId = _vkClient.UserId;

            if (audio == null || audio.OwnerId != userId)
            {
                throw new InvalidOperationException("Cannot remove specified audio");
            }

            var key = RequestHelper.CreateAlbumAudiosRequestKey(Album.AllMusicAlbumId, userId, false);

            var collection = _audiosCache.Get(key);

            var result = _vkClient.Audios.Remove(audio.Id, audio.OwnerId, audio.OwnerIsGroup ? OwnerType.Group : OwnerType.User);

            if (result)
            {
                if (collection != null)
                {
                    collection.Remove(audio);
                }

                RaiseEvent(AudioRemoved, audio);
            }

            return result;
        }

        public Audio Add(Audio audio)
        {
            var userId = _vkClient.UserId;

            if (audio == null)
            {
                throw new InvalidOperationException("Audio was not specified");
            }

            var key = RequestHelper.CreateAlbumAudiosRequestKey(Album.AllMusicAlbumId, userId, false);

            var collection = _audiosCache.Get(key);

            var result = _vkClient.Audios.Add(audio.Id, audio.OwnerId, audio.OwnerIsGroup ? OwnerType.Group : OwnerType.User);

            if (result <= 0)
            {
                return null;
            }

            var newAudio = audio.Clone();
            newAudio.Id = result;
            newAudio.IsPlayingNow = false;
            newAudio.OwnerId = userId;
            newAudio.OwnerIsGroup = false;

            if (collection != null)
            {
                collection.Insert(0, newAudio);
            }

            RaiseEvent(AudioAdded, newAudio);

            return newAudio;
        }

        public void Broadcast(Audio audio)
        {
            var audioId = 0L;
            var ownerId = 0L;
            var ownerType = OwnerType.User;

            if (audio != null)
            {
                audioId = audio.Id;
                ownerId = audio.OwnerId;
                ownerType = audio.OwnerIsGroup ? OwnerType.Group : OwnerType.User;
            }

            _vkClient.Audios.SetBroadcast(audioId, ownerId, ownerType);
        }

        public void Clear()
        {
            _audiosCache.Clean();
            _albumsCache.Clean();
            _usersCache.Clean();
            _groupsCache.Clean();
            _lyricsCache.Clean();
        }

        public void Dispose()
        {
            if (_audiosCache != null)
            {
                _audiosCache.Dispose();
            }
        }

        public void UpdateLocalization()
        {
            var allMusicAlbums = from albums in _albumsCache.Items
                                 from album in albums
                                 where album.Id == 0
                                 select album;

            foreach (var allMusicAlbum in allMusicAlbums)
            {
                allMusicAlbum.Title = VkLocalization.AllTracks;
            }

            var genre = GetGenres().Single(g => g.Id == VK.Audios.Genre.None);

            genre.Name = VK.Audios.Genre.GetGenreName(genre.Id);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Not appropriate")]
        private void RaiseEvent(EventHandler<AudioEventArgs> handler, Audio audio)
        {
            if (handler == null)
            {
                return;
            }

            handler(this, new AudioEventArgs { Audio = audio });
        }

        private static void LoadCollection<TModel, TVkModel>(GenericCache<RemoteCollection<TModel>> cache, RemoteCollection<TModel> collection, Func<RequestBase, VkCollection<TVkModel>> loadFunc, Func<TVkModel, TModel> convertFunc)
        {
            var request = collection.Request;

            request.Count = LoadItemsCount;
            request.Offset = collection.Count;

            var vkCollection = loadFunc(request);

            if (!collection.Loaded)
            {
                collection.TotalCount = vkCollection.ResponseCount;
                collection.Loaded = true;
            }

            collection.AddRange(vkCollection.Select(convertFunc));

            var key = collection.Request.CreateKey();

            cache.Set(key, collection);
        }

        private RemoteCollection<TItem> GetCollection<TItem, TRequestBase>(GenericCache<RemoteCollection<TItem>> cache, string key, Func<TRequestBase> createFunc) where TRequestBase : RequestBase
        {
            var collection = cache.GetOrAdd(key, () => new RemoteCollection<TItem>() { Request = createFunc() });

            if (!collection.Loaded)
            {
                LoadCollection(collection);
            }

            return collection;
        }

        private VkCollection<VK.Audios.Audio> LoadAudios(RequestBase request)
        {
            var count = request.Count;
            var offset = request.Offset;

            var albumAudiosRequest = request as AlbumAudiosRequest;

            if (albumAudiosRequest != null)
            {
                var ownerType = albumAudiosRequest.OwnerIsGroup ? OwnerType.Group : OwnerType.User;
                return _vkClient.Audios.GetAudio(albumAudiosRequest.OwnerId, albumAudiosRequest.AlbumId, ownerType, count: count, offset: offset);
            }

            var popularAudiosRequest = request as PopularAudiosRequest;

            if (popularAudiosRequest != null)
            {
                return _vkClient.Audios.GetPopular(popularAudiosRequest.GenreId, onlyForeign: popularAudiosRequest.OnlyForeign, count: count, offset: offset);
            }

            var recommendedAudiosRequest = request as RecommendedAudiosRequest;

            if (recommendedAudiosRequest != null)
            {
                return _vkClient.Audios.GetRecommendations(recommendedAudiosRequest.UserId, shuffle: recommendedAudiosRequest.Shuffle, count: count, offset: offset);
            }

            var searchAudiosRequest = request as SearchAudiosRequest;

            if (searchAudiosRequest != null)
            {
                return _vkClient.Audios.Search(searchAudiosRequest.Query, count: count, offset: offset);
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", request.GetType()));
        }

        private VkCollection<VK.Audios.Album> LoadAlbums(RequestBase request)
        {
            var count = request.Count;
            var offset = request.Offset - 1;

            var ownerAlbumsRequest = request as OwnerAlbumsRequest;

            if (ownerAlbumsRequest != null)
            {
                var ownerType = ownerAlbumsRequest.OwnerIsGroup ? OwnerType.Group : OwnerType.User;
                return _vkClient.Audios.GetAlbums(ownerAlbumsRequest.OwnerId, ownerType, count: count, offset: offset);
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", request.GetType()));
        }

        private static void PreviewLoadAlbums(RemoteCollection<Album> albumsCollection)
        {
            if (albumsCollection.Count != 0)
            {
                return;
            }

            var request = albumsCollection.Request;

            var ownerAlbumsRequest = request as OwnerAlbumsRequest;

            var album = new Album()
            {
                Id = Album.AllMusicAlbumId,
                Title = VkLocalization.AllTracks,
            };

            if (ownerAlbumsRequest != null)
            {
                album.OwnerId = ownerAlbumsRequest.OwnerId;
                album.OwnerIsGroup = ownerAlbumsRequest.OwnerIsGroup;
            }

            if (album.OwnerId == 0)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", request.GetType()));
            }

            albumsCollection.Add(album);
        }

        private VkCollection<VK.Users.User> LoadUsers(RequestBase request)
        {
            var userFriendsRequest = request as UserFriendsRequest;

            if (userFriendsRequest != null)
            {
                return _vkClient.Friends.Get(userFriendsRequest.UserId, count: request.Count, offset: request.Offset);
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", request.GetType()));
        }

        private VkCollection<VK.Groups.Group> LoadGroups(RequestBase request)
        {
            var userGroupsRequest = request as UserGroupsRequest;

            if (userGroupsRequest != null)
            {
                return _vkClient.Groups.Get(userGroupsRequest.UserId, count: request.Count, offset: request.Offset);
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", request.GetType()));
        }

        private static ReadOnlyCollection<Genre> LoadGenres()
        {
            var genres = from genreId in Enumerable.Range(VK.Audios.Genre.None, VK.Audios.Genre.ElectropopAndDisco)
                         where genreId != VK.Audios.Genre.Other &&
                               genreId != VK.Audios.Genre.Speech &&
                               genreId != VK.Audios.Genre.Chanson
                         let genreName = VK.Audios.Genre.GetGenreName(genreId)
                         where !string.IsNullOrEmpty(genreName)
                         select new Genre() { Id = genreId, Name = genreName };

            return new ReadOnlyCollection<Genre>(genres.ToList());
        }
    }
}
