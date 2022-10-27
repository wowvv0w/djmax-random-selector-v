using Dmrsv.Data.Interfaces;

namespace Dmrsv.Data.Context.Schema
{
    public class QueryFilter : IFilter
    {
        public List<string> ButtonTunes { get; set; } = new() { "4B", "5B", "6B", "8B" };
        public List<string> Difficulties { get; set; } = new() { "NM", "HD", "MX", "SC" };
        public List<string> Categories { get; set; } = new()
        {
            "RP", "P1", "P2", "P3", "TR", "CE", "BS", "VE", "VE2", "ES", "T1", "T2", "T3",
            "TQ", "GG", "CHU", "CY", "DM", "ESTI", "GC", "GF", "MD", "NXN"
        };
        public int[] Levels { get; set; } = { 1, 15 };
        public int[] ScLevels { get; set; } = { 1, 15 };
        public bool IncludesFavorite { get; set; } = false;
    }
}
