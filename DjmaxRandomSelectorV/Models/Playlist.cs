namespace DjmaxRandomSelectorV.Models
{
    // 3.0.0 or later
    public class Playlist
    {
        public int[] Items { get; set; }
    }

    // 2.2.0 or earlier
    public class OldPlaylist
    {
        public PlaylistItem[] Playlist { get; set; }
    }
}
