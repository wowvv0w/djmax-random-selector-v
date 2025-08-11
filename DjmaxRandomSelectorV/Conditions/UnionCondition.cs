using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record UnionCondition(IEnumerable<ICondition> Conditions) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Conditions.Any(cond => cond.IsSatisfiedBy(pattern));
        }
    }
}
