namespace DjmaxRandomSelectorV.RandomSelector
{
    public record TrackInfo
    {
        public string Title { get; init; } = string.Empty;
        public string Composer { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public TrackUserTags UserTags { get; init; } = default;
    }
}
