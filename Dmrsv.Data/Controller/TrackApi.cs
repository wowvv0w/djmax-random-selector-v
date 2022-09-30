using CsvHelper;
using CsvHelper.Configuration;
using Dmrsv.Data.Context.Core;
using Dmrsv.Data.DataTypes;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.Data.Controller
{
    public class TrackApi : DmrsvDataContext
    {
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";
        private const string AllTrackListPath = "Data/AllTrackList.csv";

        public void DownloadAllTrackList()
        {
            using var client = new HttpClient();
            string result = client.GetStringAsync(AllTrackListUrl).Result;

            using var writer = new StreamWriter(AllTrackListPath);
            writer.Write(result);
        }

        public IEnumerable<Track> GetAllTrackList()
        {
            using var reader = new StreamReader(AllTrackListPath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TrackMap>();
            var records = csv.GetRecords<Track>();

            return records.ToList();
        }
    }

    public sealed class TrackMap : ClassMap<Track>
    {
        public TrackMap()
        {
            Map(m => m.Title).Name("Title");
            Map(m => m.Category).Name("Category");
            Map(m => m.Patterns).Name(new string[16]
            {
                "4BNM", "4BHD", "4BMX", "4BSC",
                "5BNM", "5BHD", "5BMX", "5BSC",
                "6BNM", "6BHD", "6BMX", "6BSC",
                "8BNM", "8BHD", "8BMX", "8BSC"
            });
        }
    }
}
