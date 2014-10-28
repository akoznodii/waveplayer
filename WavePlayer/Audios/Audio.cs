using WavePlayer.Media;

namespace WavePlayer.Audios
{
    public class Audio : Track
    {
        public long Id { get; set; }

        public long OwnerId { get; set; }

        public long LyricsId { get; set; }

        public bool OwnerIsGroup { get; set; }

        public Audio Clone()
        {
            return MemberwiseClone() as Audio;
        }
    }
}
