using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    public record PlaylistItem
    {
        public int PatternId { get; init; }
        public string Title { get; init; }
        public string Composer { get; init; }
        public string Category { get; init; }
        public string Style { get; init; }
        public string Level { get; init; }
    }
}
