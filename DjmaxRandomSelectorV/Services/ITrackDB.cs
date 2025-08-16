using System.Collections.Generic;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public interface ITrackDB
    {
        IEnumerable<Track> AllTrack { get; }
        IEnumerable<Track> Playable { get; }
        IReadOnlyList<Category> Categories { get; }
        Track Find(int trackId);
        Pattern Find(PatternId patternId);
    }
}
