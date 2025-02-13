namespace Dmrsv.RandomSelector
{
    public record Track
    {
        public int Id { get; init; } = -1;
        public string Title { get; init; } = string.Empty;
        public string Composer { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public Dictionary<string, int> PatternLevelTable { get; init; } = new();

        public IEnumerable<Pattern> GetPatterns()
        {
            var patterns = from p in PatternLevelTable
                           select new Pattern(Id, p.Key, p.Value);
            return patterns;
        }

        public Pattern GetPatternFromId(int patternId)
        {
            var p = new Pattern(patternId, 0);
            return p with { Level = PatternLevelTable[p.Style] };
        }

        public bool EqualsTrackId(int patternId)
        {
            return Id == patternId / 100;
        }
    }
}
