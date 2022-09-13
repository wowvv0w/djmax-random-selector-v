using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models.Interfaces;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public class SelectiveFilter : IFilter
    {
        public List<PlaylistItem> Playlist { get; set; } = new();
    }
}
