using System.Collections.Generic;

namespace Dmrsv.Data.DataTypes
{
    public class Track
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public Dictionary<string, int> Patterns { get; set; }
            = new Dictionary<string, int>();
    }
}
