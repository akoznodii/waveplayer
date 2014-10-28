namespace WavePlayer.Requests
{
    public class RecommendedAudiosRequest : RequestBase
    {
        public long UserId
        {
            get; set;
        }

        public bool Shuffle
        {
            get; set;
        }
    }
}
