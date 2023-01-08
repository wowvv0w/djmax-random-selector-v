namespace Dmrsv.RandomSelector
{
    public interface IFilterHandler<TFilter> where TFilter : IFilter
    {
        IEnumerable<Music> Filter(IEnumerable<Track> trackList, TFilter filter);
    }
}
