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

            if (!recentExcluded.Any() && Recent.Count > 0)
            {
                string title = Recent.Dequeue();
                recentExcluded = from music in musicList
                                 where music.Title == title
                                 select music;
            }

            var selected = base.Select(recentExcluded);

            if (selected is not null)
            {
                Recent.Enqueue(selected.Title);
            }

            return selected;
        }
    }
}
