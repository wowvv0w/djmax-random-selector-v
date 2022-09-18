using System.Collections.Generic;

namespace Dmrsv.Data.Interfaces
{
    public interface IProvider
    {
        void Provide(Music selectedMusic, List<Track> tracks, int delay);
    }
}
