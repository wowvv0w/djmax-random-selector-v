using System.Collections.Generic;
using System.Linq;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record ButtonCondition : ICondition
    {
        public IReadOnlySet<ButtonTunes> Buttons { get; }

        public ButtonCondition(IEnumerable<ButtonTunes> buttons)
        {
            Buttons = buttons.ToHashSet();
        }

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return Buttons.Contains(pattern.Button);
        }
    }
}
