namespace Dmrsv.RandomSelector
{
    public class SelectorWithHistory : SelectorBase
    {
        public IHistory<int> History { get; set; }

        public SelectorWithHistory(IHistory<int> recent)
        {
            History = recent;
        }

        public override Pattern? Select(IList<Pattern> patternList)
        {
            var recentExcluded = from p in patternList
                                 where !History.Contains(p.TrackId)
                                 select p;

            while (!recentExcluded.Any() && History.Count > 0)
            {
                int trackId = History.Dequeue();
                recentExcluded = from p in patternList
                                 where p.TrackId == trackId
                                 select p;
            }

            var selected = base.Select(recentExcluded.ToList());

            if (selected is not null)
            {
                History.Enqueue(selected.TrackId);
            }

            return selected;
        }
    }
}
