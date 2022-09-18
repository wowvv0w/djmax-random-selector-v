using Dmrsv.Data.DataTypes;
using Dmrsv.Data.Interfaces;

namespace Dmrsv.Data.Context.Schema
{
    public class SelectiveFilter : IFilter
    {
        public List<PlaylistItem> Playlist { get; set; } = new();
    }
}
