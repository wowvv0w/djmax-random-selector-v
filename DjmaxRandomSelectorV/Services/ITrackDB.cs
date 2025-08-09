using System.Collections.Generic;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public interface ITrackDB
    {
        IReadOnlyList<Track> AllTrack { get; }
        IReadOnlyList<Track> Playable { get; }
        IReadOnlyList<Category> Categories { get; }
        Track Find(int trackId);

    }
}
