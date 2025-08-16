using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    public class FavoriteItem
    {
        public int TrackId { get; set; }
        public MusicInfo Info { get; set; }
        public bool IsPlayable { get; set; }
        public int Status { get; set; }

        public string Title => Info.Title;
        public string Composer => Info.Composer;
        public string Category => Info.Category;
    }
}
