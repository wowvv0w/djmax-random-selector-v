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


        // Obsolete
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
