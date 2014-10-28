using WavePlayer.Common;

namespace WavePlayer.Audios
{
    public class Album : ModelBase
    {
        public const long AllMusicAlbumId = 0;

        private string _title;

        public long Id { get; set; }

        public long OwnerId { get; set; }

        public bool OwnerIsGroup { get; set; }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                SetField(ref _title, value);
            }
        }
    }
}
