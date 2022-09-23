using Dmrsv.Data.Enums;

namespace Dmrsv.Data.Context.Schema
{
    internal class Config
    {
        internal int RecentsCount { get; set; } = 5;
        internal Mode Mode { get; set; } = Mode.Freestyle;
        internal Aider Aider { get; set; } = Aider.Off;
        internal Level Level { get; set; } = Level.Off;
        internal string FilterType { get; set; } = nameof(ConditionalFilter);
        internal int InputDelay { get; set; } = 30;
        internal bool SavesRecents { get; set; } = false;
        internal List<string> OwnedDlcs { get; set; } = new();
        internal double[] Position { get; set; } = Array.Empty<double>();
        internal List<string> Exclusions { get; set; } = new();
        internal List<string> Favorite { get; set; } = new();
        internal int AllTrackVersion { get; set; } = 0;
    }
}
