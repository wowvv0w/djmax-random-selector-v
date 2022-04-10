using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes
{
    public class Online : ISifter
    {
        public List<Music> Sift(List<Track> trackList, List<string> styles, int[] levels)
        {
            var musicList = from track in trackList
                            where track.Patterns.Any(x => styles.Contains(x.Key)
                                                    && x.Value >= levels[0]
                                                    && x.Value <= levels[1])
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
