using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Models
{
    public class HistoryItem
    {
        public int Number { get; set; }
        public MusicInfo Info { get; set; }
        public string Category { get; set; }
        public string Style { get; set; }
        public string Level { get; set; }
        public string Time { get; set; }

        public string ButtonTunes => Style[..2];
        public string Difficulty => Style[2..4];
    }
}
