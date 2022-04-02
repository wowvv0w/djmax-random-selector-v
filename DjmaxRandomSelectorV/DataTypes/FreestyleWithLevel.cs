using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes.Interfaces;
using DjmaxRandomSelectorV.Models;
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

        public FreestyleWithLevel(Level level)
        {
            switch (level)
            {
                case Level.Beginner:
                    _isBeginner = true;
                    break;
                case Level.Master:
                    _isBeginner = false;
                    break;
                default:
                    throw new ArgumentException("level must be either Beginner or Master.", "level");
            }
        }

        public List<Music> Sift(List<Track> trackList, List<string> styles, Filter filter)
        {
            var musicList = from track in trackList
                            from bt in filter.ButtonTunes
                            let _styles = styles.FindAll(x => x.Contains(bt))
                            let _patterns = from pattern in track.Patterns
                                            where _styles.Contains(pattern.Key)
                                            && pattern.Value >= filter.Levels[0]
                                            && pattern.Value <= filter.Levels[1]
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
