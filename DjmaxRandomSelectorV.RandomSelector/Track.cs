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

        public bool IsPlayable => UserTags.HasFlag(TrackUserTags.Playable);
        public bool IsFavorite => UserTags.HasFlag(TrackUserTags.Favorite);
        public bool IsBlacklist => UserTags.HasFlag(TrackUserTags.Blacklist);
    }
}
