namespace Dmrsv.RandomSelector
{
    public interface ISelector
    {
        Pattern? Select(IList<Pattern> patternList);
    }
}
