namespace Dmrsv.RandomSelector
{
    public record MusicInfo
    {
        public int Id { get; init; } = -1;
        public string Title { get; init; } = string.Empty;
        public string Composer { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
    }
}
