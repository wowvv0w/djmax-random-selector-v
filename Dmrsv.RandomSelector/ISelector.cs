namespace Dmrsv.RandomSelector
{
    public interface ISelector
    {
        Music? Select(IList<Music> musicList);
    }
}
