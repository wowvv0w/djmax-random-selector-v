using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Extractors
{
    public class UniquePatternExtractor : IGroupwiseExtractor
    {
        public IEnumerable<Pattern> Extract(IEnumerable<Pattern> patterns)
        {
            return Filter.GroupwiseExtract(patterns, p => p.TrackId, g => g.First());
        }
    }
}
