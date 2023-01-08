namespace Dmrsv.RandomSelector
{
    public class FilterOption
    {
        public int Except { get; set; } = 5;
        public MusicForm Mode { get; set; } = default;
        public InputMethod Aider { get; set; } = default;
        public LevelPreference Level { get; set; } = default;
    }
}
