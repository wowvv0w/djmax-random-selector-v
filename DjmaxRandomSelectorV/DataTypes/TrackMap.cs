using CsvHelper.Configuration;

namespace DjmaxRandomSelectorV.DataTypes
{
    public sealed class TrackMap : ClassMap<Track>
    {
        public TrackMap()
        {
            Map(m => m.Title).Name("Title");
            Map(m => m.Category).Name("Category");
            Map(m => m.Patterns).Name(new string[16]
            {
                "4BNM", "4BHD", "4BMX", "4BSC",
                "5BNM", "5BHD", "5BMX", "5BSC",
                "6BNM", "6BHD", "6BMX", "6BSC",
                "8BNM", "8BHD", "8BMX", "8BSC"
            });
        }
    }
}
