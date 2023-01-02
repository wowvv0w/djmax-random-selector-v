namespace Dmrsv.Data
{
    public interface ISifter
    {
        void ChangeMethod(FilterOption filterOption);
        List<Music> Sift(List<Track> tracks, IFilter filterToConvert);
    }
}
