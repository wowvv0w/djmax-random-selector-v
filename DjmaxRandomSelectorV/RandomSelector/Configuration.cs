using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public class Configuration
    {
        public FilterOption FilterOption { get; set; } = new();
        public SelectorOption SelectorOption { get; set; } = new();
        public List<string> Exclusions { get; set; } = new();
        public List<string> Favorites { get; set; } = new();
        public double[] Position { get; set; } = null;
        public int AllTrackVersion { get; set; } = 0;
    }
}
