using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class HistoryItem
    {
        private static int _number = 0;
        public int Number { get; set; }
        public string Title { get; set; }
        public string Style { get; set; }
        public int Level { get; set; }
        public string Time { get; set; }

        public HistoryItem(Music music)
        {
            _number++;
            Number = _number;
            Title = music.Title;
            Style = music.Style;
            Level = music.Level;
            Time = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
