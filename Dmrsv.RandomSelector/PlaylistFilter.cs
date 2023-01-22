using System.Collections.ObjectModel;

namespace Dmrsv.RandomSelector
{
    public class PlaylistFilter : FilterBase
    {
        public ObservableCollection<Music> Items { get; set; }

        public PlaylistFilter()
        {
            Items = new();
            Items.CollectionChanged += (s, e) => IsUpdated = true;
        }

        public override IEnumerable<Music> Filter(IEnumerable<Track> trackList)
        {
            var musicList = from item in Items
                            let track = trackList.FirstOrDefault(t => t.Title == item.Title)
                            where track is not null
                            let level = track.Patterns[item.GetStyle()]
                            select item with { Level = level };
            
            if (OutputMethod is not null)
            {
                musicList = OutputMethod.Invoke(musicList);
            }
            return musicList;
        }
    }
}
