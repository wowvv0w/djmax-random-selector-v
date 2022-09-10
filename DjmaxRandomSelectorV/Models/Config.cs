using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public class Config
    {
        public FilterOption FilterOption { get; set; } = new();
        public SelectorOption SelectorOption { get; set; } = new();
        public List<string> Exclusions { get; set; } = new();
        public double[] Position { get; set; } = null;
        public int AllTrackVersion { get; set; } = 0;
    }
}
