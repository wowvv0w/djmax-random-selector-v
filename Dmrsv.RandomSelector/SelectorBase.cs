namespace Dmrsv.RandomSelector
{
    public abstract class SelectorBase : ISelector
    {
        public virtual Music? Select(IList<Music> musicList)
        {
            if (!musicList.Any() || musicList == null)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(musicList.Count);
            return musicList[index];
        }
    }
}
