using System;
using VK.Audios;
using WavePlayer.Groups;
using WavePlayer.Requests;
using WavePlayer.Users;
using Album = WavePlayer.Audios.Album;
using Audio = WavePlayer.Audios.Audio;

namespace WavePlayer.Providers
{
    public static class ModelExtensions
    {
        public static Audio ToModel(this VK.Audios.Audio audio)
        {
            return new Audio
            {
                Id = audio.Id,
                OwnerId = audio.OwnerId,
                Artist = audio.Artist.Trim(),
                Title = audio.Title.Trim(),
                Duration = TimeSpan.FromSeconds(audio.Duration),
                Source = new Uri(audio.Url),
                OwnerIsGroup = audio.OwnerType == OwnerType.Group,
                LyricsId = audio.LyricsId
            };
        }

        public static Album ToModel(this VK.Audios.Album album)
        {
            return new Album
            {
                Id = album.Id,
                OwnerId = album.OwnerId,
                Title = album.Title.Trim(),
                OwnerIsGroup = album.OwnerType == OwnerType.Group,
            };
        }

        public static User ToModel(this VK.Users.User user)
        {
            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Photo = new Uri(user.Photo),
            };
        }

        public static Group ToModel(this VK.Groups.Group group)
        {
            return new Group()
            {
                Id = group.Id,
                Name = group.Name,
                GroupType = group.GroupType,
                Photo = new Uri(group.Photo50),
            };
        }

        public static AlbumAudiosRequest ToRequest(this Album album)
        {
            return new AlbumAudiosRequest
            {
                AlbumId = album.Id,
                OwnerId = album.OwnerId,
                OwnerIsGroup = album.OwnerIsGroup,
            };
        }
    }
}
