namespace Dmrsv.RandomSelector
{
    public interface IFilter
    {
        bool IsUpdated { get; }
        IEnumerable<Pattern> Filter(IEnumerable<Track> trackList);
    }
}
