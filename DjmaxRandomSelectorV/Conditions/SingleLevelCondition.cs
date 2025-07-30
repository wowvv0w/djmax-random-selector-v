using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record SingleLevelCondition(bool IsSC, int Level) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            bool isSCPattern = pattern.Difficulty == Difficulty.SC;
            return IsSC == isSCPattern && Level == pattern.Level;
        }
    }
}
