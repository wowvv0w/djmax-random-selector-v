using Dmrsv.Data;

namespace Dmrsv.RandomSelector
{
    public interface ISelector
    {
        Music Select(IEnumerable<Music> musicList);
    }
}
