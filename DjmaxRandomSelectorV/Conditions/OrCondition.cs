using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record OrCondition : ICompoundCondition, ICondition
    {
        public IEnumerable<ICondition> Conditions { get; }

        public OrCondition(IEnumerable<ICondition> conditions)
        {
            Conditions = conditions.ToList();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Conditions.Any(cond => cond.IsSatisfiedBy(pattern));
        }
    }
}
