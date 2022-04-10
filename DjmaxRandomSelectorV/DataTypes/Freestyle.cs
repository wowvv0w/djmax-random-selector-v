using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes
{
    public class Freestyle : ISifter
    {
        public List<Music> Sift(List<Track> trackList, List<string> styles, int[] levels)
        {
            var musicList = from track in trackList
                            from pattern in track.Patterns
                            where styles.Contains(pattern.Key)
                            && pattern.Value >= levels[0]
                            && pattern.Value <= levels[1]
                            select new Music
                            {
                                Title = track.Title,
                                Style = pattern.Key,
                                Level = pattern.Value.ToString()
                            };
            return musicList.ToList();
        }
    }
}
