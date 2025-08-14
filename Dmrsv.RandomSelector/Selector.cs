namespace Dmrsv.RandomSelector
{
    public static class Selector
    {
        public static Pattern? RandomSelect(IReadOnlyList<Pattern> candidates)
        {
            if (!candidates.Any() || candidates is null)
            {
                return null;
            }

            int selectedIndex = new Random().Next(candidates.Count);
            return candidates[selectedIndex];
        }
    }
}
