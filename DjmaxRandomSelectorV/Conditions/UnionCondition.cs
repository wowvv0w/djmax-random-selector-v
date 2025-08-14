using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record UnionCondition : IMergedCondition, ICondition
    {
        public IEnumerable<ICondition> Conditions { get; }

        public UnionCondition(IEnumerable<ICondition> conditions)
        {
            Conditions = conditions.ToList();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Conditions.Any(cond => cond.IsSatisfiedBy(pattern));
        }
    }
}
