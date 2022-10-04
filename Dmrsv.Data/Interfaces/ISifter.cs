using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.DataTypes;

namespace Dmrsv.Data.Interfaces
{
    public interface ISifter
    {
        void ChangeMethod(FilterOption filterOption);
        List<Music> Sift(List<Track> tracks, IFilter filterToConvert);
    }
}
