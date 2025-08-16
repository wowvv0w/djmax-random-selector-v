using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    // TODO: renewal
    public class AdvancedFilter : IFilterState
    {
        public event Action OnStateChanged;

        private readonly ITrackDB _trackDB;

        public ObservableCollection<Pattern> PatternList { get; }

        public AdvancedFilter(ITrackDB trackDB)
        {
            _trackDB = trackDB;
            PatternList = new ObservableCollection<Pattern>();
            Initialize();
        }

        public AdvancedFilter(ITrackDB trackDB, IEnumerable<Pattern> patterns)
        {
            _trackDB = trackDB;
            PatternList = new ObservableCollection<Pattern>(patterns);
            Initialize();
        }
        
        private void Initialize()
        {
            PatternList.CollectionChanged += (s, e) => OnStateChanged?.Invoke();
        }

        public ICondition ToCondition()
        {
            var result = PatternList.Where(p => _trackDB.Find(p.TrackId)?.IsPlayable ?? false).Select(p => p.Id);
            return new PatternIdCondition(result);
        }
    }
}
