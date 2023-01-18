using Dmrsv.RandomSelector;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Messages
{
    public class SettingMessage
    {
        public FilterType FilterType { get; set; }
        public int InputInterval { get; set; }
        public bool SavesExclusion { get; set; }
        public List<string> OwnedDlcs { get; set; }
    }
}
