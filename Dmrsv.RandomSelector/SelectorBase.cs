namespace Dmrsv.RandomSelector
{
    public abstract class SelectorBase : ISelector
    {
        public virtual Pattern? Select(IList<Pattern> patternList)
        {
            if (!patternList.Any() || patternList == null)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(patternList.Count);
            return patternList[index];
        }
    }
}
