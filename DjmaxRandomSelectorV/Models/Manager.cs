using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DjmaxRandomSelectorV.Properties;

namespace DjmaxRandomSelectorV.Models
{
    public class Manager
    {
        public static void ReadAllTrackList()
        {
            var userData = @"C:\Projects\DjmaxRandomSelectorV\DjmaxRandomSelectorV\DataFiles\test_data.csv";

            using (var reader = new StreamReader(userData, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<MusicMap>();
                var records = csv.GetRecords<Track>().ToList();
                Selector.AllTrackList = records;
            }
        }

        public static void UpdateTrackList()
        {
            var ownedDlc = Settings.Default.ownedDlc;
            var titleFilter = CreateTitleFilter();
            var trackList = from track in Selector.AllTrackList
                            where ownedDlc.Contains(track.Category)
                            && titleFilter.Contains(track.Title) == false
                            select track;
            Selector.TrackList = trackList.ToList();

            List<string> CreateTitleFilter()
            {
                var list = new List<string>();

                if (ownedDlc.Contains("P3") == false)
                {
                    list.Add("glory day (Mintorment Remix)");
                    list.Add("glory day -JHS Remix-");
                }
                if (ownedDlc.Contains("TR") == false) list.Add("Nevermind");
                if (ownedDlc.Contains("CE") == false) list.Add("Rising The Sonic");
                if (ownedDlc.Contains("BS") == false) list.Add("ANALYS");
                if (ownedDlc.Contains("T1") == false) list.Add("Do you want it");
                if (ownedDlc.Contains("T2") == false) list.Add("End of Mythology");
                if (ownedDlc.Contains("T3") == false) list.Add("ALiCE");

                if (ownedDlc.Contains("CE") && !ownedDlc.Contains("BS") && !ownedDlc.Contains("T1"))
                    list.Add("Here in the Moment ~Extended Mix~");
                if (!ownedDlc.Contains("CE") && ownedDlc.Contains("BS") && !ownedDlc.Contains("T1"))
                    list.Add("Airwave ~Extended Mix~");
                if (!ownedDlc.Contains("CE") && !ownedDlc.Contains("BS") && ownedDlc.Contains("T1"))
                    list.Add("SON OF SUN ~Extended Mix~");
                if (!ownedDlc.Contains("VE") && ownedDlc.Contains("VE2"))
                    list.Add("너로피어오라 ~Original Ver.~");

                return list;
            }  
        }
    }
}
