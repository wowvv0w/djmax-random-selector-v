namespace DjmaxRandomSelectorV.RandomSelector
{
    public record Track
    {
        public int Id { get; init; } = -1;
        public MusicInfo Info { get; init; } = new();
        public Pattern[] Patterns { get; init; } = Array.Empty<Pattern>();
        public TrackUserTags UserTags { get; init; } = TrackUserTags.None;

        public string Title => Info.Title;
        public string Composer => Info.Composer;
        public string Category => Info.Category;

        public bool IsPlayable => UserTags == TrackUserTags.Playable;
        public bool IsFavorite => UserTags == TrackUserTags.Favorite;
        public bool IsBlacklist => UserTags == TrackUserTags.Blacklist;
    }
}
