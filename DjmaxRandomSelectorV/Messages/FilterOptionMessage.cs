using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Messages
{
    public record FilterOptionMessage(int RecentsCount, MusicForm MusicForm, InputMethod InputMethod, LevelPreference LevelPreference)
    {
        public FilterOptionMessage(IFilterOptionState filterOption)
            : this(filterOption.RecentsCount, filterOption.Mode, filterOption.Aider, filterOption.Level) { }
    }
}
