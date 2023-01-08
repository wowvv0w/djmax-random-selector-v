namespace Dmrsv.RandomSelector
{
    public record Music
    {
        public string Title { get; init; } = string.Empty;
        public string ButtonTunes { get; init; } = string.Empty;
        public string Difficulty { get; init; } = string.Empty;
        public int Level { get; init; } = default;
    }
}
