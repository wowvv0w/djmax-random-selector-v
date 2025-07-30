using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public interface ICondition
    {
        bool IsSatisfiedBy(Pattern pattern);
    }
}
