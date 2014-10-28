namespace WavePlayer.Requests
{
    public class PopularAudiosRequest : RequestBase
    {
        public int GenreId { get; set; }

        public bool OnlyForeign { get; set; }
    }
}
