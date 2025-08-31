using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;
using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    public class BasicFilter : IFilterState
    {
        public event Action OnStateChanged;

        public ObservableCollection<string> ButtonTunes { get; }
        public ObservableCollection<string> Difficulties { get; }
        public ObservableCollection<string> Categories { get; }
        public ObservableCollection<int> Levels { get; }
        public ObservableCollection<int> ScLevels { get; }

        private bool _includesFavorite;
        public bool IncludesFavorite
        {
            get { return _includesFavorite; }
            set
            {
                _includesFavorite = value;
                OnStateChanged?.Invoke();
            }
        }

        public BasicFilter()
        {
            ButtonTunes = new ObservableCollection<string>() { "4B", "5B", "6B", "8B" };
            Difficulties = new ObservableCollection<string>() { "NM", "HD", "MX", "SC" };
            Categories = new ObservableCollection<string>();
            Levels = new ObservableCollection<int>() { 1, 15 };
            ScLevels = new ObservableCollection<int>() { 1, 15 };
            _includesFavorite = false;
            Initialize();
        }

        public BasicFilter(Dmrsv3BasicFilterPreset filter)
        {
            ButtonTunes = new ObservableCollection<string>(filter.ButtonTunes);
            Difficulties = new ObservableCollection<string>(filter.Difficulties);
            Categories = new ObservableCollection<string>(filter.Categories);
            Levels = new ObservableCollection<int>(filter.Levels);
            ScLevels = new ObservableCollection<int>(filter.ScLevels);
            _includesFavorite = filter.IncludesFavorite;
            Initialize();
        }

        private void Initialize()
        {
            foreach (var o in GetType().GetProperties())
            {
                var observable = o.GetValue(this, null) as INotifyCollectionChanged;
                if (observable is not null)
                {
                    observable.CollectionChanged += (s, e) => OnStateChanged?.Invoke();
                }
            }
        }

        public ICondition ToCondition()
        {
            var categoryCond = Condition.Or(
                Categories.Any() ? new CategoryCondition(Categories) : null,
                IncludesFavorite ? new TrackUserTagsCondition(TrackUserTags.Favorite) : null
            );
            var levelCond = Condition.Or(
                Difficulties.Contains("NM") ? new RangeLevelCondition(false, Levels[0], Levels[1]) : null,
                Difficulties.Contains("SC") ? new RangeLevelCondition(true, ScLevels[0], ScLevels[1]) : null
            );
            var resultCond = Condition.And(
                categoryCond,
                Condition.Not(new TrackUserTagsCondition(TrackUserTags.Blacklist)),
                ButtonTunes.Any() ? new ButtonCondition(ButtonTunes.Select(bt => bt.AsButtonTunes())) : Condition.Null,
                levelCond
            );
            return resultCond;
        }
    }
}
