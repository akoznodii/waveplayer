namespace WavePlayer.Media
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tracklist", Justification = "Correct name")]
    public interface ITracklist
    {
        int Count { get; }

        int IndexOf(Track item);

        Track this[int index] { get; }
    }
}
