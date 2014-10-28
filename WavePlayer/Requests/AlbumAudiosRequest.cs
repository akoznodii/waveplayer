namespace WavePlayer.Requests
{
    public class AlbumAudiosRequest : RequestBase
    {
        public long AlbumId { get; set; }

        public long OwnerId { get; set; }

        public bool OwnerIsGroup { get; set; }
    }
}
