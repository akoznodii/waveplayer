namespace WavePlayer.Requests
{
    public class OwnerAlbumsRequest : RequestBase
    {
        public long OwnerId { get; set; }

        public bool OwnerIsGroup { get; set; }
    }
}
