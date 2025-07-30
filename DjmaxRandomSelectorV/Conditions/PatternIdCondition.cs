using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record PatternIdCondition(ICollection<int> PatternIds) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return PatternIds.Contains(pattern.PatternId);
        }
    }
}
