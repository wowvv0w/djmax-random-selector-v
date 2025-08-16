namespace Dmrsv.RandomSelector
{
    public record Track
    {
        public int Id { get; init; } = -1;
        public MusicInfo Info { get; init; } = new();
        public Pattern[] Patterns { get; init; } = Array.Empty<Pattern>();
        public bool IsPlayable { get; init; } = false;

        public string Title => Info.Title;
        public string Composer => Info.Composer;
        public string Category => Info.Category;
    }
}
