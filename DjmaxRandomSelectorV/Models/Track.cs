using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Track
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public Dictionary<string, int> Patterns { get; set; }
            = new Dictionary<string, int>();
    }
}
