namespace Dmrsv.RandomSelector
{
    public interface ILocator
    {
        bool StartsAutomatically { get; set; }
        int InputInterval { get; set; }
        bool InvokesInput { get; set; }

        void Locate(Music music, IEnumerable<Track> trackList);
    }
}
