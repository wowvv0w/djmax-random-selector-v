using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record RangeLevelCondition(bool IsSC, int MinLevel, int MaxLevel) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            bool isSCPattern = pattern.Difficulty == Difficulty.SC;
            return IsSC == isSCPattern && MinLevel <= pattern.Level && pattern.Level <= MaxLevel;
        }
    }
}
