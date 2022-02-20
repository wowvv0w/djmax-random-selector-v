using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public class Filter
    {
        public List<string> ButtonTunes { get; set; } = new List<string>();
        public List<string> Difficulties { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
        public int[] Levels { get; set; } = new int[2] { 1, 15 }; /// 1 ~ 15
        public List<string> Recents { get; set; } = new List<string>();
    }
}
