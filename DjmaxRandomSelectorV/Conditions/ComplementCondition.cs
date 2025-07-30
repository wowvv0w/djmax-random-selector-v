using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record ComplementCondition(ICondition Condition) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return !Condition.IsSatisfiedBy(pattern);
        }
    }
}
