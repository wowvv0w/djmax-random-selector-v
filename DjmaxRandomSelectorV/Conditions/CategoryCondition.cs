using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record CategoryCondition : ICondition
    {
        public IReadOnlySet<string> Categories { get; }

        public CategoryCondition(IEnumerable<string> categories)
        {
            Categories = categories.ToHashSet();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Categories.Contains(pattern.Info.Category);
        }
    }
}
