using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.States;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    public class BasicFilter : IFilterState
    {
        public event Action OnStateChanged;

        private readonly ISettingStateManager _settingManager;

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

        public BasicFilter(ISettingStateManager settingManager)
        {
            _settingManager = settingManager;
            ButtonTunes = new ObservableCollection<string>() { "4B", "5B", "6B", "8B" };
            Difficulties = new ObservableCollection<string>() { "NM", "HD", "MX", "SC" };
            Categories = new ObservableCollection<string>();
            Levels = new ObservableCollection<int>() { 1, 15 };
            ScLevels = new ObservableCollection<int>() { 1, 15 };
            Initialize();
        }

        public BasicFilter(Dmrsv3BasicFilterPreset filter, ISettingStateManager settingManager)
        {
            _settingManager = settingManager;
            ButtonTunes = new ObservableCollection<string>(filter.ButtonTunes);
            Difficulties = new ObservableCollection<string>(filter.Difficulties);
            Categories = new ObservableCollection<string>(filter.Categories);
            Levels = new ObservableCollection<int>(filter.Levels);
            ScLevels = new ObservableCollection<int>(filter.ScLevels);
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
            if (!ButtonTunes.Any() || !Difficulties.Any() || (!Categories.Any() && !IncludesFavorite))
            {
                return Condition.Null;
            }

            var setting = _settingManager.GetSetting();
            var categoryCond = Condition.CreateUnion(
                (Categories.Any(), () => new CategoryCondition(Categories.ToHashSet())),
                (IncludesFavorite, () => new TrackIdCondition(setting.Favorite))
            );
            var levelCond = Condition.CreateUnion(
                (Difficulties.Contains("NM"), () => new RangeLevelCondition(false, Levels[0], Levels[1])),
                (Difficulties.Contains("SC"), () => new RangeLevelCondition(true, ScLevels[0], ScLevels[1]))
            );
            var resultCond = Condition.CreateIntersection(
                (true, () => categoryCond),
                (setting.Blacklist.Any(), () => Condition.ComplementOf(new TrackIdCondition(setting.Blacklist.ToHashSet()))),
                (true, () => new ButtonCondition(ButtonTunes.Select(bt => bt.AsButtonTunes()).ToHashSet())),
                (true, () => levelCond)
            );
            return resultCond;
        }
    }
}
