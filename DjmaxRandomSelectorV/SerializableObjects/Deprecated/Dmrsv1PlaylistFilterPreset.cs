namespace DjmaxRandomSelectorV.SerializableObjects.Deprecated
{
    public class Dmrsv1PlaylistFilterPreset
    {
        public Dmrsv1PlaylistItem[] Playlist { get; set; }

        public class Dmrsv1PlaylistItem
        {
            public string Title { get; set; }
            public string Style { get; set; }
        }
    }
}
