namespace Dmrsv.RandomSelector
{
    public static class Filter
    {
        public static IEnumerable<Track> Sift(IEnumerable<Track> tracks, Func<Track, bool> trackCondition)
        {
            return tracks.Where(t => trackCondition(t));
        }

        public static IEnumerable<Pattern> Sift(IEnumerable<Pattern> patterns, Func<Pattern, bool> patternCondition)
        {
            return patterns.Where(p => patternCondition(p));
        }

        public static IEnumerable<Pattern> Sift(IEnumerable<Track> tracks, Func<Track, bool> trackCondition, Func<Pattern, bool> patternCondition)
        {
            var patterns = Sift(tracks, trackCondition).SelectMany(t => t.Patterns);
            return Sift(patterns, patternCondition);
        }

        public static IEnumerable<Pattern> GroupwiseExtract<TGroupKey>(IEnumerable<Pattern> patterns, Func<Pattern, TGroupKey> groupKeySelector, Func<IEnumerable<Pattern>, Pattern> resultSelector)
        {
            return patterns.GroupBy(groupKeySelector).Select(g => resultSelector(g));
        }
    }
}
