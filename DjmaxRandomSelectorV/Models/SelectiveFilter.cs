using DjmaxRandomSelectorV.DataTypes;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public class SelectiveFilter : Filter
    {
        public List<Music> Playlist { get; set; } = new();
    }
}
