using CsvHelper;
using DjmaxRandomSelectorV.Properties;
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
        private const string ALL_TRACK_LIST = "DataFiles/AllTrackList.csv";
        private const string PRESET_DEFAULT = "DataFiles/default.json";

        private const string VERSION_URL = "";
        private const string ALL_TRACK_URL = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/DataFiles/AllTrackList.csv";

        public static void SelectorUpdateCheck()
        {

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

        public static void UpdateAllTrackList()
        {
            string data;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                data = client.DownloadString(ALL_TRACK_URL);    
            }

            using (var writer = new StreamWriter(ALL_TRACK_LIST))
            {
                writer.Write(data);
            }
        }

        public static void UpdateTrackList()
        {
            var ownedDlcs = Settings.Default.ownedDlcs;
            var basicCategories = new string[] { "RP", "P1", "P2", "GG" };
            ownedDlcs.AddRange(basicCategories);

            var titleFilter = CreateTitleFilter();
            var trackList = from track in Selector.AllTrackList
                            where ownedDlcs.Contains(track.Category)
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
        public static Filter LoadPreset(string path = PRESET_DEFAULT)
        {
            Filter filter;

            try
            {
                using (var reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    filter = JsonSerializer.Deserialize<Filter>(json);
                }
            }
            catch
            {
                filter = new Filter();
            }

            return filter;
        }

        public static void SavePreset(Filter filter, string path = PRESET_DEFAULT)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true, IgnoreReadOnlyProperties = false };
            string jsonString = JsonSerializer.Serialize(filter, options);

            using (var writer = new StreamWriter(path))
            {
                writer.Write(jsonString);
            }
        }
    }
}
