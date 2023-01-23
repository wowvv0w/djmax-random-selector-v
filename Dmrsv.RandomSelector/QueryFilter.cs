using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public class QueryFilter : FilterBase
    {
        private List<string> _favorites;
        private List<string> _blacklist;

        public ObservableCollection<string> ButtonTunes { get; set; } 
        public ObservableCollection<string> Difficulties { get; set; } 
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<int> Levels { get; set; }
        public ObservableCollection<int> ScLevels { get; set; }

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
            _favorites = new();
            _blacklist = new();
            ButtonTunes = new() { "4B", "5B", "6B", "8B" };
            Difficulties = new() { "NM", "HD", "MX", "SC" };
            Categories = new() { "RP", "P1", "P2", "GG" };
            Levels = new() { 1, 15 };
            ScLevels = new() { 1, 15 };

            foreach (var o in GetType().GetProperties())
            {
                var observable = o.GetValue(this, null) as INotifyCollectionChanged;
                if (observable is not null)
                {
                    observable.CollectionChanged += (s, e) => IsUpdated = true;
                }
            }
        }

        public override IEnumerable<Music> Filter(IEnumerable<Track> trackList)
        {
            var musicList = from t in trackList
                            where Categories.Contains(t.Category) || (Categories.Contains("FAVORITE") && Favorites.Contains(t.Title))
                            where !Blacklist.Contains(t.Category)
                            from m in t.GetMusicList()
                            where ButtonTunes.Contains(m.ButtonTunes) && Difficulties.Contains(m.Difficulty)
                            let levels = m.Difficulty == "SC" ? ScLevels : Levels
                            where levels[0] <= m.Level && m.Level <= levels[1]
                            select m;

            if (OutputMethod is not null)
            {
                musicList = OutputMethod.Invoke(musicList);
            }

            IsUpdated = false;
            return musicList;
        }
    }
}
