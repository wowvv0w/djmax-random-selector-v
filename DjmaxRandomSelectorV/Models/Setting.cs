using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Setting
    {
        // Equipment
        public int RecentsCount { get; set; } = 5;
        public Aider Aider { get; set; } = Aider.Off;

        // Setting
        public int InputDelay { get; set; } = 30;
        public bool SavesRecents { get; set; } = false;
        public List<string> OwnedDlcs { get; set; } = new List<string>();
        public double[] Position { get; set; } = new double[2];
        
        // Inventory
        public List<string> Favorite { get; set; } = new List<string>();
        
        // Version
        public int AllTrackVersion { get; set; } = 0;
    }
}
