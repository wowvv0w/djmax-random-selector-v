using System.Collections.Generic;

namespace DjmaxRandomSelectorV.SerializableObjects
{
    public class Dmrsv3BasicFilterPreset
    {
        public List<string> ButtonTunes { get; set; }
        public List<string> Difficulties { get; set; }
        public List<string> Categories { get; set; }
        public List<int> Levels { get; set; }
        public List<int> ScLevels { get; set; }
        public bool IncludesFavorite { get; set; }
    }
}
