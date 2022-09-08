using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.RandomSelector.Interfaces
{
    public interface ISifter
    {
        public void ChangeMethod(FilterOption filterOption);
        public List<Music> Sift(List<Track> tracks, Filter filterToConvert);
    }
}
