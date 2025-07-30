using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record ButtonCondition(ICollection<ButtonTunes> Buttons) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Buttons.Contains(pattern.Button);
        }
    }
}
