using System.Collections.Generic;
using DjmaxRandomSelectorV.Enums;

namespace DjmaxRandomSelectorV.States
{
    public interface ISettingState
    {
        FilterType FilterType { get; set; }
        int InputDelay { get; set; }
        List<int> Favorite { get; set; }
        List<int> Blacklist { get; set; }
        List<string> OwnedDlcs { get; set; }
        bool SavesRecents { get; set; }
        uint StartKeyCode { get; set; }
    }
}
