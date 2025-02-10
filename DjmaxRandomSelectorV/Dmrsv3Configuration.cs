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
        public Dmrsv3Setting Setting { get; set; }
        public Dmrsv3FilterOption FilterOption { get; set; }
        public Dmrsv3WindowProperty WindowProperty { get; set; }
        public Dmrsv3VersionInfo VersionInfo { get; set; }

        public class Dmrsv3Setting
        {
            public FilterType FilterType { get; set; }
            public int InputInterval { get; set; }
            public List<int> Favorite { get; set; }
            public List<int> Blacklist { get; set; }
            public List<string> OwnedDlcs { get; set; }
            public bool SavesRecent { get; set; }
            public List<int> RecentPlayed { get; set; }
            public uint StartKeyCode { get; set; }
        }

        public class Dmrsv3FilterOption
        {
            public int RecentsCount { get; set; }
            public MusicForm MusicForm { get; set; }
            public InputMethod InputMethod { get; set; }
            public LevelPreference LevelPreference { get; set; }
        }

        public class Dmrsv3WindowProperty
        {
            public double[] Position { get; set; }
            public double[] Size { get; set; }
        }

        public class Dmrsv3VersionInfo
        {
            public int AllTrackVersion { get; set; }
        }
    }
}
