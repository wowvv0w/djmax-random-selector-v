using System;
using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Extractors
{
    public class GroupwiseExtractorBuilder : IGroupwiseExtractorBuilder
    {
        public MusicForm StyleType { get; set; }
        public LevelPreference LevelPreference { get; set; }

        public IGroupwiseExtractor Build()
        {
            return (StyleType, LevelPreference) switch
            {
                (MusicForm.Free, _) => new Extractor<int>(p => p.TrackId, g => g.First()),
                (MusicForm.Default, LevelPreference.Lowest) => new Extractor<PerButtonGroup>(p => new(p.TrackId, p.Button), g => g.First()),
                (MusicForm.Default, LevelPreference.Highest) => new Extractor<PerButtonGroup>(p => new(p.TrackId, p.Button), g => g.Last()),
                _ => null
            };
        }

        private record PerButtonGroup(int TrackId, ButtonTunes Button);

        private class Extractor<TGroupKey> : IGroupwiseExtractor
        {
            private Func<IEnumerable<Pattern>, IEnumerable<Pattern>> _extract;

            public Extractor(Func<Pattern, TGroupKey> groupKeySelector, Func<IEnumerable<Pattern>, Pattern> resultSelector)
            {
                _extract = patterns => Filter.GroupwiseExtract(patterns, groupKeySelector, resultSelector);
            }

            public IEnumerable<Pattern> Extract(IEnumerable<Pattern> patterns)
            {
                return _extract(patterns);
            }
        }
    }
}
