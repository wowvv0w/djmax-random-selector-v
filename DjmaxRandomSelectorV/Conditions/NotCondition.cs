using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record NotCondition(ICondition Condition) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return !Condition.IsSatisfiedBy(pattern);
        }
    }
}
