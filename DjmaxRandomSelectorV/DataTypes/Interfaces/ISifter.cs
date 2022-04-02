using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes.Interfaces
{
    public interface ISifter
    {
        List<Music> Sift(List<Track> trackList, List<string> styles, int[] levels);
    }
}
