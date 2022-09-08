using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public abstract class Filter
    {
        public List<string> Exclusions { get; set; } = new();
    }
}
