using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Models.Interfaces;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.RandomSelector.Interfaces
{
    public interface ISifter
    {
        string CurrentMethod { get; }
        void ChangeMethod(FilterOption filterOption);
        void SetMethod(string methodName);
        List<Music> Sift(List<Track> tracks, IFilter filterToConvert);
    }
}
