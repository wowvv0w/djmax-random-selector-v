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
        public List<Music> Sift(List<Track> trackList, List<string> styles, int[] levels, int[] scLevels)
        {
            var musicList = from track in trackList
                            where track.Patterns.Any(x => styles.Contains(x.Key)
                                                    && (
                                                        (!x.Key.Contains("SC") && x.Value >= levels[0] && x.Value <= levels[1])
                                                        || (x.Key.Contains("SC") && x.Value >= scLevels[0] && x.Value <= scLevels[1])
                                                       ))
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
