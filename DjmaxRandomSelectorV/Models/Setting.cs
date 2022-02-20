using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Setting
    {
        // Additional Filter
        public bool AutoStart { get; set; } = false;
        public int RecentsCount { get; set; } = 5;
        public List<string> Favorite { get; set; } = new List<string>();

        // Setting
        public int InputDelay { get; set; } = 30;
        public bool SavesRecents { get; set; } = false;
        public List<string> OwnedDlcs { get; set; } = new List<string>();
        public double[] Position { get; set; } = new double[2];
        
        // Version
        public int AllTrackVersion { get; set; } = 0;
    }
}
