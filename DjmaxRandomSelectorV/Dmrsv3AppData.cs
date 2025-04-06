using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DjmaxRandomSelectorV.Models;

namespace DjmaxRandomSelectorV
{
    public class Dmrsv3AppData
    {
        public string[] CategoryType { get; set; }
        public string[] BasicCategories { get; set; }
        public Category[] Categories { get; set; }
        public (int Id, string[][] RequiredDlc)[] LinkDisc { get; set; }
        public string GameWindowTitle { get; set; }
    }
}
