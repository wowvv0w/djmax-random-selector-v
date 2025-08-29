using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record IntersectionCondition : IMergedCondition, ICondition
    {
        public IEnumerable<ICondition> Conditions { get; }

        public IntersectionCondition(IEnumerable<ICondition> conditions)
        {
            Conditions = conditions.ToList();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Conditions.All(cond => cond.IsSatisfiedBy(pattern));
        }
    }
}
