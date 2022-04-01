using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Online : ISifter
    {
        public List<Music> Sift(List<Track> trackList, List<string> styles, Filter filter)
        {
            var musicList = from track in trackList
                            where track.Patterns.Any(x => styles.Contains(x.Key)
                                                    && x.Value >= filter.Levels[0]
                                                    && x.Value <= filter.Levels[1])
                            select new Music
                            {
                                Title = track.Title,
                                Style = "FREE",
                                Level = "-"
                            };
            return musicList.ToList();
        }
    }
}
