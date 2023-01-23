namespace Dmrsv.RandomSelector
{
    public abstract class SelectorBase : ISelector
    {
        public virtual Music? Select(IEnumerable<Music> musicList)
        {
            if (!musicList.Any() || musicList == null)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(musicList.Count() - 1);
            return musicList.ElementAt(index);
        }
    }
}
