using DjmaxRandomSelectorV.DataTypes;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.RandomSelector.Interfaces
{
    public interface IProvider
    {
        void Provide(Music selectedMusic, List<Track> tracks, int delay);
    }
}
