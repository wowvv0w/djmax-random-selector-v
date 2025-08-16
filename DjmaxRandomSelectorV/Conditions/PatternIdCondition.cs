using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record PatternIdCondition : ICondition
    {
        public IReadOnlySet<PatternId> PatternIds { get; }

        public PatternIdCondition(IEnumerable<PatternId> patternIds)
        {
            PatternIds = patternIds.ToHashSet();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return PatternIds.Contains(pattern.Id);
        }
    }
}
