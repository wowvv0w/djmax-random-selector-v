using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class InputCommand
    {
        public char ButtonTune { get; set; }
        public char Initial { get; set; }
        public int VerticalInputCount { get; set; }
        public int RightInputCount { get; set; }
        public bool IsForward { get; set; }
        public bool IsAlphabet { get; set; }
    }
}
