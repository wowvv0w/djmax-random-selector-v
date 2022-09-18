using System.Collections.Generic;

namespace Dmrsv.Data.Interfaces
{
    public interface ISifter
    {
        string CurrentMethod { get; }
        void ChangeMethod(FilterOption filterOption);
        void SetMethod(string methodName);
        List<Music> Sift(List<Track> tracks, IFilter filterToConvert);
    }
}
