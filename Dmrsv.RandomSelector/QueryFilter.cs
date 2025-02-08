using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public class QueryFilter : FilterBase
    {
        private ObservableCollection<string> _buttonTunes;
        private ObservableCollection<string> _difficulties;
        private ObservableCollection<string> _categories;
        private ObservableCollection<int> _levels;
        private ObservableCollection<int> _scLevels;
        private bool _includesFavorite;
        private List<string> _favorites;
        private List<string> _blacklist;

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
        public List<string> Favorites
        {
            get { return _favorites; }
            set
            {
                _favorites = value;
                IsUpdated = true;
            }
        }
        [JsonIgnore]
        public List<string> Blacklist
        {
            get { return _blacklist; }
            set
            {
                _blacklist = value;
                IsUpdated = true;
            }
        }

        public QueryFilter()
        {
            _buttonTunes = new ObservableCollection<string>() { "4B", "5B", "6B", "8B" };
            _difficulties = new ObservableCollection<string>() { "NM", "HD", "MX", "SC" };
            _categories = new ObservableCollection<string>() { "RP", "P1", "P2", "GG" };
            _levels = new ObservableCollection<int>() { 1, 15 };
            _scLevels = new ObservableCollection<int>() { 1, 15 };
            _favorites = new List<string>();
            _blacklist = new List<string>();

            foreach (var o in GetType().GetProperties())
            {
                var observable = o.GetValue(this, null) as INotifyCollectionChanged;
                if (observable is not null)
                {
                    observable.CollectionChanged += (s, e) => IsUpdated = true;
                }
            }
        }

        public override List<Music> Filter(IEnumerable<OldTrack> trackList)
        {
            var musicList = from t in trackList
                            where Categories.Contains(t.Category) || (IncludesFavorite && Favorites.Contains(t.Title))
                            where !Blacklist.Contains(t.Title)
                            from m in t.GetMusicList()
                            let styles = (from b in ButtonTunes
                                          from d in Difficulties
                                          select $"{b}{d}")
                            where styles.Contains(m.Style)
                            let levels = m.Difficulty == "SC" ? ScLevels : Levels
                            where levels[0] <= m.Level && m.Level <= levels[1]
                            select m;

            if (OutputMethod is not null)
            {
                musicList = OutputMethod.Invoke(musicList);
            }

            IsUpdated = false;
            return musicList.ToList();
        }
    }
}
