using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using WavePlayer.Audios;
using WavePlayer.Groups;
using WavePlayer.Providers;
using WavePlayer.Users;

#if DESIGN_DATA

namespace WavePlayer.UI.DesignTime
{
    internal class VkDataProvider : IVkDataProvider
    {
        private readonly Lazy<ICollection<Genre>> _genres = new Lazy<ICollection<Genre>>(LoadGenres, true);
        private readonly Lazy<ICollection<User>> _friends = new Lazy<ICollection<User>>(LoadFriends, true);
        private readonly Lazy<ICollection<Group>> _groups = new Lazy<ICollection<Group>>(LoadGroups, true);
        private readonly Lazy<ICollection<Album>> _albums = new Lazy<ICollection<Album>>(LoadAlbums, true);
        private readonly Lazy<ICollection<Audio>> _audios = new Lazy<ICollection<Audio>>(LoadAudios, true);
        private static readonly Random Random = new Random(1024);

        public event EventHandler<AudioEventArgs> AudioAdded { add { } remove { } }

        public event EventHandler<AudioEventArgs> AudioRemoved { add { } remove { } }

        public ICollection<Genre> GetGenres()
        {
            return _genres.Value;
        }

        public ICollection<User> GetUserFriends(User user)
        {
            return _friends.Value;
        }

        public ICollection<Group> GetUserGroups(User user)
        {
            return _groups.Value;
        }

        public ICollection<Album> GetUserAlbums(User user)
        {
            return _albums.Value;
        }

        public ICollection<Album> GetGroupAlbums(Group group)
        {
            return _albums.Value;
        }

        public ICollection<Audio> GetAlbumAudios(Album album)
        {
            return _audios.Value;
        }

        public ICollection<Audio> GetPopularAudios(Genre genre, bool onlyForeign)
        {
            return _audios.Value;
        }

        public ICollection<Audio> GetRecommendedAudios(User user, bool shuffle)
        {
            return _audios.Value;
        }

        public ICollection<Audio> GetSearchAudios(string query)
        {
            return _audios.Value;
        }

        public void LoadCollection<T>(ICollection<T> collection)
        {
        }

        public void Clear()
        {
        }

        public void UpdateLocalization()
        {
        }

        public bool Remove(Audio audio)
        {
            return false;
        }

        public Audio Add(Audio audio)
        {
            return null;
        }

        public Lyrics GetLyrics(Audio audio)
        {
            return new Lyrics()
            {
                Artist = "Artist",
                Id = 1,
                Title = "Title",
                Text = "This is lyrics text\nThis is lyrics text\nThis is lyrics text\n"
            };
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

        private static ReadOnlyCollection<User> LoadFriends()
        {
            var friends = from id in Enumerable.Range(2, 15)
                          select new User()
                          {
                              Id = id,
                              FirstName = string.Format(CultureInfo.InvariantCulture, "Clone{0}", id),
                              LastName = "Stormtrooper",
                              Photo = new Uri(@"http://cs425117.vk.me/v425117621/5fda/qT5hj6GDC2E.jpg")
                          };

            return new ReadOnlyCollection<User>(friends.ToList());
        }

        private static ReadOnlyCollection<Group> LoadGroups()
        {
            var groups = from id in Enumerable.Range(1, 15)
                         select new Group()
                         {
                             Id = id,
                             Name = string.Format(CultureInfo.InvariantCulture, "Star Wars {0}", id),
                             Photo = new Uri(@"http://cs617328.vk.me/v617328478/93aa/pqwx8u-Qids.jpg")
                         };

            return new ReadOnlyCollection<Group>(groups.ToList());
        }

        private static ReadOnlyCollection<Audio> LoadAudios()
        {
            var audios = from id in Enumerable.Range(1, 25)
                         select new Audio()
                         {
                             Id = id,
                             Title = string.Format(CultureInfo.InvariantCulture, "Star Wars track {0}", id),
                             Artist = "The Stormtroopers",
                             OwnerId = 1,
                             Duration = TimeSpan.FromSeconds(Random.Next(90, 360))
                         };

            var result = new ReadOnlyCollection<Audio>(audios.ToList());

            result.First().IsPlayingNow = true;

            return result;
        }

        private static ReadOnlyCollection<Album> LoadAlbums()
        {
            var albums = from id in Enumerable.Range(0, 15)
                         select new Album()
                         {
                             Id = id,
                             Title = string.Format(CultureInfo.InvariantCulture, "Star Wars {0}", id),
                         };

            return new ReadOnlyCollection<Album>(albums.ToList());
        }
    }
}
#endif
