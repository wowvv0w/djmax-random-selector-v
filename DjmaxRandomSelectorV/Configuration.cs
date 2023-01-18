using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV
{
    public class Configuration
    {
        public int RecentsCount { get; set; } = 5;
        public MusicForm Mode { get; set; } = MusicForm.Default;
        public InputMethod Aider { get; set; } = InputMethod.Default;
        public LevelPreference Level { get; set; } = LevelPreference.None;
        public FilterType FilterType { get; set; } = FilterType.Query;
        public int InputDelay { get; set; } = 30;
        public bool SavesRecents { get; set; } = false;
        public List<string> OwnedDlcs { get; set; } = new();
        public double[] Position { get; set; } = Array.Empty<double>();
        public List<string> Exclusions { get; set; } = new();
        public List<string> Favorite { get; set; } = new();
        public List<string> Blacklist { get; set; } = new();
        public int AllTrackVersion { get; set; } = 0;
        public string SavePath { get; set; } = @"Data\config.json";
    }
}
