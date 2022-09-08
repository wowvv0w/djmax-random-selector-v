using CsvHelper;
using DjmaxRandomSelectorV.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public class TrackHandler
    {
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";
        private const string AllTrackListPath = "Data/AllTrackList.csv";

        private List<Track> _allTrackList;

        private void DownloadAllTrackList()
        {
            using var client = new HttpClient();
            string result = client.GetStringAsync(AllTrackListUrl).Result;

            using var writer = new StreamWriter(AllTrackListPath);
            writer.Write(result);
        }
        private void GetAllTrackList()
        {
            using var reader = new StreamReader(AllTrackListPath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TrackMap>();
            var records = csv.GetRecords<Track>().ToList();

            _allTrackList = records;
        }
        public List<Track> UpdateTrackList(List<string> ownedDlcs)
        {
            var categories = ownedDlcs.Concat(new List<string>() { "RP", "P1", "P2", "GG" });
            var titleFilter = CreateTitleFilter(ownedDlcs);
            var trackList = from track in _allTrackList
                            where categories.Contains(track.Category)
                            && !titleFilter.Contains(track.Title)
                            select track;

            return trackList.ToList();
        }
        private List<string> CreateTitleFilter(List<string> ownedDlcs)
        {
            var list = new List<string>();

            if (!ownedDlcs.Contains("P3"))
            {
                list.Add("glory day (Mintorment Remix)");
                list.Add("glory day -JHS Remix-");
            }
            if (!ownedDlcs.Contains("TR"))
                list.Add("Nevermind");
            if (!ownedDlcs.Contains("CE"))
                list.Add("Rising The Sonic");
            if (!ownedDlcs.Contains("BS"))
                list.Add("ANALYS");
            if (!ownedDlcs.Contains("T1"))
                list.Add("Do you want it");
            if (!ownedDlcs.Contains("T2"))
                list.Add("End of Mythology");
            if (!ownedDlcs.Contains("T3"))
                list.Add("ALiCE");
            if (!ownedDlcs.Contains("TQ"))
                list.Add("Techno Racer");
            if (ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Here in the Moment ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Airwave ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && ownedDlcs.Contains("T1"))
                list.Add("SON OF SUN ~Extended Mix~");
            if (!ownedDlcs.Contains("VE") && ownedDlcs.Contains("VE2"))
                list.Add("너로피어오라 ~Original Ver.~");

            return list;
        }
    }
}
