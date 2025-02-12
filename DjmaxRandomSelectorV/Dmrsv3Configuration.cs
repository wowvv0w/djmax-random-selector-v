using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV
{
    public class Dmrsv3Configuration
    {
        public Dmrsv3Setting Setting { get; set; } = new();
        public Dmrsv3FilterOption FilterOption { get; set; } = new();
        public Dmrsv3WindowProperty WindowProperty { get; set; } = new();
        public Dmrsv3VersionInfo VersionInfo { get; set; } = new();

        public class Dmrsv3Setting
        {
            public FilterType FilterType { get; set; } = FilterType.Query;
            public int InputInterval { get; set; } = 30;
            public List<int> Favorite { get; set; } = new();
            public List<int> Blacklist { get; set; } = new();
            public List<string> OwnedDlcs { get; set; } = new();
            public bool SavesRecent { get; set; } = false;
            public List<int> RecentPlayed { get; set; } = new();
            public uint StartKeyCode { get; set; } = 118;
        }

        public class Dmrsv3FilterOption
        {
            public int RecentsCount { get; set; } = 5;
            public MusicForm MusicForm { get; set; } = MusicForm.Default;
            public InputMethod InputMethod { get; set; } = InputMethod.Default;
            public LevelPreference LevelPreference { get; set; } = LevelPreference.None;
        }

        public class Dmrsv3WindowProperty
        {
            public double[] Position { get; set; } = Array.Empty<double>();
            public double[] Size { get; set; } = Array.Empty<double>();
        }

        public class Dmrsv3VersionInfo
        {
            public int AllTrackVersion { get; set; } = 0;
        }
    }
}
