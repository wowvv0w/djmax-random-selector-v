namespace DjmaxRandomSelectorV.RandomSelector
{
    public record Track
    {
        public int Id { get; init; } = -1;
        public TrackInfo Info { get; init; } = new();
        public Pattern[] Patterns { get; init; } = Array.Empty<Pattern>();

        public string Title => Info.Title;
        public string Composer => Info.Composer;
        public string Category => Info.Category;
        public TrackUserTags UserTags => Info.UserTags;

        public bool IsPlayable => UserTags == TrackUserTags.Playable;
        public bool IsFavorite => UserTags == TrackUserTags.Favorite;
        public bool IsBlacklist => UserTags == TrackUserTags.Blacklist;
    }
}
