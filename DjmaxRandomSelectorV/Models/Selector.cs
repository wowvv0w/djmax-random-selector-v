using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public static class Selector
    {

        public static void Start()
        {

        }
        public static List<Music> SiftOut(List<Track> trackList)
        {
            List<string> Styles = new List<string>();
            foreach(string button in Filter.ButtonTunes)
            {
                foreach(string difficulty in Filter.Difficulties)
                {
                    Styles.Add($"{button}{difficulty}");
                }
            }
            var musicList = from track in trackList
                            from pattern in track.Patterns
                            where Filter.Categories.Contains(track.Category)
                            && Styles.Contains(pattern.Key)
                            && pattern.Value >= Filter.Levels[0]
                            && pattern.Value <= Filter.Levels[1]
                            select new Music
                            {
                                Title = track.Title,
                                Style = pattern.Key,
                                Level = pattern.Value
                            };
            return musicList.ToList();
        }

        public static void Pick()
        {

        }
    }
}
