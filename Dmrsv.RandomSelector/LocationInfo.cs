namespace Dmrsv.RandomSelector
{
    public record LocationInfo
    {
        public int TrackId { get; init; } = -1;
        public char Group { get; init; } = '#';
        public int Index { get; init; } = 0;
        public Dictionary<string, int> DifficultyOrder { get; init; } = new();
    }
}
