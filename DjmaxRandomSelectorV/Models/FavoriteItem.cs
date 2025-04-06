using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    public class FavoriteItem
    {
        public int Id { get; init; } = -1;
        public string Title { get; init; } = string.Empty;
        public string Composer { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public bool IsPlayable { get; init; } = false;

        public FavoriteItem()
        {
        }

        public FavoriteItem(Track track)
        {
            Id = track.Id;
            Title = track.Title;
            Composer = track.Composer;
            Category = track.Category;
            IsPlayable = true;
        }
    }
}
