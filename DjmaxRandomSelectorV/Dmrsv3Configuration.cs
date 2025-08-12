using System;
using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV
{
    public class Dmrsv3Configuration
    {
        /*************** Setting *****************/
        public FilterType FilterType { get; set; } = FilterType.Query;
        public int InputDelay { get; set; } = 30;
        public List<int> Favorite { get; set; } = new();
        public List<int> Blacklist { get; set; } = new();
        public List<string> OwnedDlcs { get; set; } = new();
        public bool SavesRecents { get; set; } = false;
        public List<int> RecentPlayed { get; set; } = new();
        public uint StartKeyCode { get; set; } = 118;

        /*************** Filter Option *****************/
        public int RecentsCount { get; set; } = 5;
        public MusicForm Mode { get; set; } = MusicForm.Default;
        public InputMethod Aider { get; set; } = InputMethod.Default;
        public LevelPreference Level { get; set; } = LevelPreference.None;


        /*************** Window Property *****************/
        public double[] Position { get; set; } = Array.Empty<double>();
        public double[] Size { get; set; } = Array.Empty<double>();


        /*************** Version Info *****************/
        public long AllTrackVersion { get; set; } = 0;
        public string AppdataVersion { get; set; } = string.Empty;
    }
}
