namespace Dmrsv.RandomSelector
{
    public interface IFilterOld
    {
        bool IsUpdated { get; }
        IEnumerable<Pattern> Filter(IEnumerable<Track> trackList);
    }
}
