namespace Dmrsv.RandomSelector
{
    public interface IFilter
    {
        bool IsUpdated { get; }
        Func<IEnumerable<Music>, IEnumerable<Music>>? OutputMethod { get; set; }
        IEnumerable<Music> Filter(IEnumerable<Track> trackList);
    }
}
