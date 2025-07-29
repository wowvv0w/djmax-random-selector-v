using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    interface ICondition
    {
        bool IsEnabled { get; }
        bool IsSatisfiedBy(Pattern pattern);
    }
}
