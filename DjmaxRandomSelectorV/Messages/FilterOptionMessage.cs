using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Messages
{
    public class FilterOptionMessage
    {
        public int Except { get; set; }
        public MusicForm Mode { get; set; }
        public InputMethod Aider { get; set; }
        public LevelPreference Level { get; set; }
    }
}
