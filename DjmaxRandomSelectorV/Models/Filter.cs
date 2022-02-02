using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public static class Filter
    {
        public static List<string> ButtonTunes { get; set; } = new List<string>();
        public static List<string> Difficulties { get; set; } = new List<string>();
        public static List<string> Categories { get; set; } = new List<string>();
        public static int[] Levels { get; set; } = new int[2] { 1, 15 }; /// 1 ~ 15
        public static bool Mode { get; set; } = true; /// FREESTYLE = true, ONLINE = false
        public static int InputDelay { get; set; } = 50;
    }
}
