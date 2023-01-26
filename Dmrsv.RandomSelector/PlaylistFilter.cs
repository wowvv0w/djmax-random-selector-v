using System.Collections.ObjectModel;

namespace Dmrsv.RandomSelector
{
    public class PlaylistFilter : FilterBase
    {
        private ObservableCollection<Music> _items;
        public ObservableCollection<Music> Items
        {
            get => _items;
            set
            {
                _items = value;
                _items.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }

        public PlaylistFilter()
        {
            _items = new ObservableCollection<Music>();
            _items.CollectionChanged += (s, e) => IsUpdated = true;
        }

        public override List<Music> Filter(IEnumerable<Track> trackList)
        {
            var musicList = from item in Items
                            let track = trackList.FirstOrDefault(t => t.Title == item.Title)
                            where track is not null
                            let level = track.Patterns[item.Style]
                            select item with { Level = level };
            
            if (OutputMethod is not null)
            {
                musicList = OutputMethod.Invoke(musicList);
            }

            IsUpdated = false;
            return musicList.ToList();
        }
    }
}
