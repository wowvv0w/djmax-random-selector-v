using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes.Interfaces
{
    public interface IProvider
    {
        void Provide(Music selectedMusic, List<Track> trackList, int delay);
    }
}
