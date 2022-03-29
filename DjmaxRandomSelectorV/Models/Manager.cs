using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DjmaxRandomSelectorV.Models
{
    public class Manager
    {
        private const string ALL_TRACK_LIST = "Data/AllTrackList.csv";
        
        private const string PRESET_DEFAULT = "Default";

        private const string VERSION_URL = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        private const string ALL_TRACK_URL = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";

        public static (int, int) GetLastVersions()
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadString(VERSION_URL);
                var versions = data.Split(',');
                
                var lastSelectorVer = Int32.Parse(versions[0]);
                var lastAllTrackVer = Int32.Parse(versions[1]);

                return (lastSelectorVer, lastAllTrackVer);
            }
        }
        
        public static void UpdateAllTrackList()
        {
            string data;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                data = client.DownloadString(ALL_TRACK_URL);    
            }

            using (var writer = new StreamWriter(ALL_TRACK_LIST))
            {
                writer.Write(data);
            }
        }

        public static void ReadAllTrackList()
        {
            using (var reader = new StreamReader(ALL_TRACK_LIST, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<TrackMap>();
                var records = csv.GetRecords<Track>().ToList();
                Selector.AllTrackList = records;
            }
        }

        public static void UpdateTrackList(List<string> ownedDlcs)
        {
            var basicCategories = new List<string>() { "RP", "P1", "P2", "GG" };
            var titleFilter = CreateTitleFilter();
            var trackList = from track in Selector.AllTrackList
                            where (ownedDlcs.Contains(track.Category) || basicCategories.Contains(track.Category))
                            && titleFilter.Contains(track.Title) == false
                            select track;

            Selector.TrackList = trackList.ToList();

            List<string> CreateTitleFilter()
            {
                var list = new List<string>();

                if (ownedDlcs.Contains("P3") == false)
                {
                    list.Add("glory day (Mintorment Remix)");
                    list.Add("glory day -JHS Remix-");
                }
                if (ownedDlcs.Contains("TR") == false) { list.Add("Nevermind"); }
                if (ownedDlcs.Contains("CE") == false) { list.Add("Rising The Sonic"); }
                if (ownedDlcs.Contains("BS") == false) { list.Add("ANALYS"); }
                if (ownedDlcs.Contains("T1") == false) { list.Add("Do you want it"); }
                if (ownedDlcs.Contains("T2") == false) { list.Add("End of Mythology"); }
                if (ownedDlcs.Contains("T3") == false) { list.Add("ALiCE"); }

                if (ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                {
                    list.Add("Here in the Moment ~Extended Mix~");
                }
                if (!ownedDlcs.Contains("CE") && ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                {
                    list.Add("Airwave ~Extended Mix~");
                }
                if (!ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && ownedDlcs.Contains("T1"))
                {
                    list.Add("SON OF SUN ~Extended Mix~");
                }
                if (!ownedDlcs.Contains("VE") && ownedDlcs.Contains("VE2"))
                {
                    list.Add("너로피어오라 ~Original Ver.~");
                }

                return list;
            }  
        }


    }
}
