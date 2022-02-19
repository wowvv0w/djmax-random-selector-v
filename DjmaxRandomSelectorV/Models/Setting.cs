using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Setting
    {
        public int InputDelay { get; set; } = 30;
        public List<string> OwnedDlcs { get; set; } = new List<string>();
        public int AllTrackVersion { get; set; } = 0;
        public double[] Position { get; set; } = new double[2];
    }
}
