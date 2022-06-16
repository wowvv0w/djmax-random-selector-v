using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes
{
    public class FreestyleWithLevel : ISifter
    {
        private bool _isBeginner;

        public FreestyleWithLevel(string level)
        {
            switch (level)
            {
                case "BEGINNER":
                    _isBeginner = true;
                    break;
                case "MASTER":
                    _isBeginner = false;
                    break;
            }
        }

        public List<Music> Sift(List<Track> trackList, List<string> styles, int[] levels, int[] scLevels)
        {
            var buttonTunes = new string[4] { "4B", "5B", "6B", "8B" };

            var musicList = from track in trackList
                            from bt in buttonTunes
                            let _styles = styles.FindAll(x => x.Contains(bt))
                            let _patterns = from pattern in track.Patterns
                                            where _styles.Contains(pattern.Key)
                                            && (
                                                (!pattern.Key.Equals("SC") && pattern.Value >= levels[0] && pattern.Value <= levels[1])
                                                || (pattern.Key.Equals("SC") && pattern.Value >= scLevels[0] && pattern.Value <= scLevels[1])
                                               )
                                            select pattern
                            where _patterns.Count() > 0
                            let _pattern = _isBeginner ? _patterns.First() : _patterns.Last()
                            select new Music
                            {
                                Title = track.Title,
                                Style = _pattern.Key,
                                Level = _pattern.Value.ToString()
                            };
            return musicList.ToList();
        }
    }
}
