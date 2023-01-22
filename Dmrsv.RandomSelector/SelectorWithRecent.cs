namespace Dmrsv.RandomSelector
{
    public class SelectorWithRecent : SelectorBase
    {
        public IRecent<string> Recent { get; set; }

        public SelectorWithRecent(IRecent<string> recent)
        {
            Recent = recent;
        }

        public override Music? Select(IEnumerable<Music> musicList)
        {
            var recentExcluded = from music in musicList
                                 where !Recent.Contains(music.Title)
                                 select music;

            var selected = base.Select(recentExcluded);

            if (selected is not null)
            {
                Recent.Add(selected.Title);
            }

            return selected;
        }
    }
}
