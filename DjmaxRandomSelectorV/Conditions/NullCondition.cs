using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public class NullCondition : ICondition
    {
        private static NullCondition _instance = null;
        private NullCondition() { }
        public static NullCondition Instance => _instance ??= new NullCondition();

        public bool IsSatisfiedBy(Pattern pattern)
        {
            return false;
        }
    }
}
