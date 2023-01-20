using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public class QueryFilter : IFilter
    {
        public List<string> ButtonTunes { get; set; } = new() { "4B", "5B", "6B", "8B" };
        public List<string> Difficulties { get; set; } = new() { "NM", "HD", "MX", "SC" };
        public List<string> Categories { get; set; } = new()
        {
            "RP", "P1", "P2", "GG"
        };
        public int[] Levels { get; set; } = { 1, 15 };
        public int[] ScLevels { get; set; } = { 1, 15 };

        [JsonIgnore]
        public List<string> Favorites { get; set; } = new();
        [JsonIgnore]
        public List<string> Blacklist { get; set; } = new();
    }
}
