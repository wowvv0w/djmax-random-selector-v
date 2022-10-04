using Dmrsv.Data.Enums;

namespace Dmrsv.Data.Context.Schema
{
    public class Config
    {
        public int RecentsCount { get; set; } = 5;
        public Mode Mode { get; set; } = Mode.Freestyle;
        public Aider Aider { get; set; } = Aider.Off;
        public Level Level { get; set; } = Level.Off;
        public FilterType FilterType { get; set; } = FilterType.Query;
        public int InputDelay { get; set; } = 30;
        public bool SavesRecents { get; set; } = false;
        public List<string> OwnedDlcs { get; set; } = new();
        public double[] Position { get; set; } = Array.Empty<double>();
        public List<string> Exclusions { get; set; } = new();
        public List<string> Favorite { get; set; } = new();
        public int AllTrackVersion { get; set; } = 0;
    }
}
