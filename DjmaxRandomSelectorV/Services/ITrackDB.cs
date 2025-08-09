using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public interface ITrackDB
    {
        IReadOnlyList<Track> AllTrack { get; }
        IReadOnlyList<Track> Playable { get; }
        IReadOnlyList<string> Categories { get; }
        Track Find(int trackId);

    }
}
