namespace Dmrsv.Data.Context.Schema
{
    public class ExtraFilter
    {
        public List<string> Exclusions { get; set; } = new();
        public List<string> Favorites { get; set; } = new();
    }
}
