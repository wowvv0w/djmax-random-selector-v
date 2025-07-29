using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record ButtonCondition(bool IsEnabled, ICollection<ButtonTunes> Buttons) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Buttons.Contains(pattern.Button);
        }
    }
}
