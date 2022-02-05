using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Filter
    {
        public List<string> ButtonTunes { get; set; } = new List<string>();
        public List<string> Difficulties { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
        public int[] Levels { get; set; } = new int[2] { 1, 15 }; /// 1 ~ 15
        public bool Mode { get; set; } = true; /// FREESTYLE = true, ONLINE = false
    }
}
