using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record TrackIdCondition(bool IsBlacklist, ICollection<int> TrackIds) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            bool isContained = TrackIds.Contains(pattern.TrackId);
            return !IsBlacklist == isContained;
        }
    }
}
