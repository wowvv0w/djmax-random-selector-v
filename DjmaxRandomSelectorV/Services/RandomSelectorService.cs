using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.Extractors;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public class RandomSelectorService : IRandomSelector
    {
        private readonly IHistory _history;

        private IReadOnlyList<Pattern> _candidates = null;
        private Pattern _lastSelected = null;

        public delegate void SelectionCompletedEventHandler(Pattern selected);
        public event SelectionCompletedEventHandler OnSelectionCompleted;

        public RandomSelectorService(IHistory history)
        {
            _history = history;
        }

        public void SetCandidates(IEnumerable<Track> playable, ICondition condition, IGroupwiseExtractor extractor)
        {
            var patterns = playable.SelectMany(x => x.Patterns);
            if (condition is not null)
            {
                patterns = Filter.Sift(patterns, condition.IsSatisfiedBy);
            }
            if (extractor is not null)
            {
                patterns = extractor.Extract(patterns);
            }
            _candidates = patterns.ToList();
        }

        public Pattern Select()
        {
            var recentExcluded = _candidates.Where(p => !_history.Contains(p.TrackId));
            while (!recentExcluded.Any() && _history.Count > 0)
            {
                int trackId = _history.Dequeue();
                recentExcluded = _candidates.Where(p => p.TrackId == trackId);
            }
            Pattern selected = Selector.RandomSelect(recentExcluded.ToList());
            if (selected is not null)
            {
                _lastSelected = selected;
                _history.Enqueue(selected.TrackId);
                OnSelectionCompleted?.Invoke(selected);
            }
            return selected;
        }

        public Pattern Reselect()
        {
            if (_lastSelected is not null)
            {
                OnSelectionCompleted?.Invoke(_lastSelected);
            }
            return _lastSelected;
        }
    }
}
