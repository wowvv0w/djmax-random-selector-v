using Dmrsv.Data;

namespace Dmrsv.RandomSelector
{
    public class SelectorWithRecent : Selector
    {
        private readonly Queue<string> _recent;
        private int _recentMax;

        public int RecentMax
        {
            get => _recentMax;
            set
            {
                _recentMax = value;
                int diff = _recent.Count - _recentMax;
                for (int i = 0; i < diff; i++)
                {
                    _recent.Dequeue();
                }
            }
        }

        public SelectorWithRecent(IEnumerable<string>? recent = null, int recentMax = 5)
        {
            _recent = new Queue<string>(recent ?? Array.Empty<string>());
            RecentMax = recentMax;
        }

        public override Music Select(IEnumerable<Music> musicList)
        {
            var recentExcluded = from music in musicList
                                 where !_recent.Contains(music.Title)
                                 select music;

            var selected = base.Select(recentExcluded);

            _recent.Enqueue(selected.Title);
            if (_recent.Count > RecentMax)
            {
                _recent.Dequeue();
            }

            return selected;
        }

        public IEnumerable<string> CopyRecent() => _recent.Select(x => x);
    }
}
