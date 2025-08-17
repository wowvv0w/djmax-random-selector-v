using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Extractors
{
    public class EasiestPatternExtractor : IGroupwiseExtractor
    {
        public IEnumerable<Pattern> Extract(IEnumerable<Pattern> patterns)
        {
            return Filter.GroupwiseExtract(patterns, p => new { p.TrackId, p.Button }, g => g.First());
        }
    }
}
