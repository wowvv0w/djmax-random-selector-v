using System.Collections.Generic;

namespace DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible
{
    public record VArchiveDBTrack
    {
        public int Title { get; set; }
        public string Name { get; set; }
        public string Composer { get; set; }
        public string DlcCode { get; set; }
        public string Dlc { get; set; }
        public Dictionary<string, Dictionary<string, VArchiveDBPattern>> Patterns { get; set; }
    }
}
