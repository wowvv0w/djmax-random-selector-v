using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using Dmrsv.RandomSelector.Enums;

namespace Dmrsv.RandomSelector
{
    public class BasicFilter : FilterBase
    {
        private ObservableCollection<string> _buttonTunes;
        private ObservableCollection<string> _difficulties;
        private ObservableCollection<string> _categories;
        private ObservableCollection<int> _levels;
        private ObservableCollection<int> _scLevels;
        private bool _includesFavorite;
        private List<int> _favorite;
        private List<int> _blacklist;

        public ObservableCollection<string> ButtonTunes
        {
            get => _buttonTunes;
            set
            {
                _buttonTunes = value;
                _buttonTunes.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }
        public ObservableCollection<string> Difficulties
        {
            get => _difficulties;
            set
            {
                _difficulties = value;
                _difficulties.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                _categories.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }
        public ObservableCollection<int> Levels
        {
            get => _levels;
            set
            {
                _levels = value;
                _levels.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }
        public ObservableCollection<int> ScLevels
        {
            get => _scLevels;
            set
            {
                _scLevels = value;
                _scLevels.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }
        public bool IncludesFavorite
        {
            get => _includesFavorite;
            set
            {
                _includesFavorite = value;
                IsUpdated = true;
            }
        }

        [JsonIgnore]
        public List<int> Favorite
        {
            get { return _favorite; }
            set
            {
                _favorite = value;
                IsUpdated = true;
            }
        }
        [JsonIgnore]
        public List<int> Blacklist
        {
            get { return _blacklist; }
            set
            {
                _blacklist = value;
                IsUpdated = true;
            }
        }

        public BasicFilter()
        {
            _buttonTunes = new ObservableCollection<string>() { "4B", "5B", "6B", "8B" };
            _difficulties = new ObservableCollection<string>() { "NM", "HD", "MX", "SC" };
            _categories = new ObservableCollection<string>();
            _levels = new ObservableCollection<int>() { 1, 15 };
            _scLevels = new ObservableCollection<int>() { 1, 15 };
            _favorite = new List<int>();
            _blacklist = new List<int>();

            foreach (var o in GetType().GetProperties())
            {
                var observable = o.GetValue(this, null) as INotifyCollectionChanged;
                if (observable is not null)
                {
                    observable.CollectionChanged += (s, e) => IsUpdated = true;
                }
            }
        }

        public override IEnumerable<Pattern> Filter(IEnumerable<Track> trackList)
        {
            var styles = from bt in ButtonTunes
                         from df in Difficulties
                         select $"{bt}{df}";
            var musicList = from t in trackList
                            where Categories.Contains(t.Category) || (IncludesFavorite && Favorite.Contains(t.Id))
                            where !Blacklist.Contains(t.Id)
                            from p in t.Patterns
                            where styles.Contains(p.Style)
                            let levels = p.Difficulty == Difficulty.SC ? ScLevels : Levels
                            where levels[0] <= p.Level && p.Level <= levels[1]
                            select p;
            IsUpdated = false;
            return musicList.ToList();
        }
    }
}
