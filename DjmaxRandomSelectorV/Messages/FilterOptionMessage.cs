using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Messages
{
    public record FilterOptionMessage(int RecentsCount, MusicForm MusicForm, InputMethod InputMethod, LevelPreference LevelPreference);
}
