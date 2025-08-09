using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.States
{
    public interface IFilterOptionState
    {
        int RecentsCount { get; set; }
        MusicForm Mode { get; set; }
        InputMethod Aider { get; set; }
        LevelPreference Level { get; set; }
    }
}
