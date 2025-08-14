using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record TrackIdCondition : ICondition
    {
        public IReadOnlySet<int> TrackIds { get; }

        public TrackIdCondition(IEnumerable<int> trackIds)
        {
            TrackIds = trackIds.ToHashSet();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return TrackIds.Contains(pattern.TrackId);
        }
    }
}
