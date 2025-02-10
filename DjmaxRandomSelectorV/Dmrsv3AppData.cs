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
        public string DataFolderName { get; set; }
        public string OldDataFolderName { get; set; }
        public string ConfigFileName { get; set; }
        public string AllTrackFileName { get; set; }
        public string AllTrackDownloadUrl { get; set; }
        public string AllTrackDownloadUrlAlt { get; set; }
        public string CurrentFilterFileName { get; set; }
        public string VersionCheckUrl { get; set; }
        public string HomepageUrl { get; set; }
        public string[] CategoryType { get; set; }
        public string[] BasicCategories { get; set; }
        public Category[] Categories { get; set; }
        public (int Id, string[][] RequiredDlc)[] LinkDisc { get; set; }
        public string GameWindowTitle { get; set; }
    }
}
