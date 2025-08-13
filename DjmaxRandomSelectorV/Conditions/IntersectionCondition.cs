using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record IntersectionCondition(IEnumerable<ICondition> Conditions) : IMergedCondition, ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Conditions.All(cond => cond.IsSatisfiedBy(pattern));
        }
    }
}
