namespace Dmrsv.RandomSelector
{
    public interface IFilter
    {
        bool IsUpdated { get; }
        Func<IEnumerable<Music>, IEnumerable<Music>>? OutputMethod { get; set; }
        List<Music> Filter(IEnumerable<Track> trackList);
    }
}
