namespace Dmrsv.RandomSelector
{
    public class SelectorWithHistory : SelectorBase
    {
        public IHistory<string> History { get; set; }

        public SelectorWithHistory(IHistory<string> recent)
        {
            History = recent;
        }

        public override Music? Select(IEnumerable<Music> musicList)
        {
            var recentExcluded = from music in musicList
                                 where !History.Contains(music.Title)
                                 select music;

            if (!recentExcluded.Any() && History.Count > 0)
            {
                string title = History.Dequeue();
                recentExcluded = from music in musicList
                                 where music.Title == title
                                 select music;
            }

            var selected = base.Select(recentExcluded);

            if (selected is not null)
            {
                History.Enqueue(selected.Title);
            }

            return selected;
        }
    }
}
