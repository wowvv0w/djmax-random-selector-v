using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record CategoryCondition(ICollection<string> Categories) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Categories.Contains(pattern.Info.Category);
            
        }
    }
}
