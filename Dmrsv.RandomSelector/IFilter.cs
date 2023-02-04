namespace Dmrsv.RandomSelector
{
    public interface IFilter
    {
        bool IsUpdated { get; }
        OutputMethodCallback? OutputMethod { get; set; }
        List<Music> Filter(IEnumerable<Track> trackList);
    }
}
