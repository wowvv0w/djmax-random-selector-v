namespace Dmrsv.RandomSelector
{
    public record Track
    {
        public MusicInfo Info { get; init; } = new();
        public Pattern[] Patterns { get; init; } = Array.Empty<Pattern>();

        public int Id => Info.Id;
        public string Title => Info.Title;
        public string Composer => Info.Composer;
        public string Category => Info.Category;
    }
}
