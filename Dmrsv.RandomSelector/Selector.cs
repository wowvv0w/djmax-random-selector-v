namespace Dmrsv.RandomSelector
{
    public static class Selector
    {
        public static Pattern? SelectFrom(IReadOnlyList<Pattern> candidates, int index)
        {
            if (candidates is null)
            {
                return null;
            }
            if (index < 0 || index >= candidates.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return candidates[index];
        }
    }
}
