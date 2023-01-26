using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Dmrsv.RandomSelector
{
    public class TrackManager
    {
        private const string AllTrackUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";
        private const string AllTrackPath = @"Data\AllTrackList.csv";

        private readonly Dictionary<string, string[]> _dlcMusicInRespect = new()
        {
            ["P3"] = new[] { "glory day (Mintorment Remix)", "glory day -JHS Remix-" },
            ["TR"] = new[] { "Nevermind" },
            ["CE"] = new[] { "Rising The Sonic" },
            ["BS"] = new[] { "ANALYS" },
            ["T1"] = new[] { "Do you want it" },
            ["T2"] = new[] { "End of Mythology" },
            ["T3"] = new[] { "ALiCE" },
            ["TQ"] = new[] { "Techno Racer" },
        };
        private readonly Dictionary<Predicate<IEnumerable<string>>, string> _linkDiscMusic = new()
        {
            [x => x.Contains("CE") && (x.Contains("BS") || x.Contains("T1"))] = "Here in the Moment ~Extended Mix~",
            [x => x.Contains("BS") && (x.Contains("CE") || x.Contains("T1"))] = "Airwave ~Extended Mix~",
            [x => x.Contains("T1") && (x.Contains("CE") || x.Contains("BS"))] = "SON OF SUN ~Extended Mix~",
            [x => x.Contains("VE2") && x.Contains("VE")] = "너로피어오라 ~Original Ver.~",
        };

        public void DownloadAllTrack()
        {
            using var client = new HttpClient();
            string result = client.GetStringAsync(AllTrackUrl).Result;

            using var writer = new StreamWriter(AllTrackPath);
            writer.Write(result);
        }

        public List<Track> GetAllTrack()
        {
            using var reader = new StreamReader(AllTrackPath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TrackMap>();
            var records = csv.GetRecords<Track>();

            return records.ToList();
        }

        public List<Track> CreateTracks(IEnumerable<string> dlcs)
        {
            var allTrack = GetAllTrack();
            var basicCategories = new string[] { "RP", "P1", "P2", "GG" };
            var categories = dlcs.Concat(basicCategories);

            var exclusions = new List<string>();
            exclusions.AddRange(_dlcMusicInRespect.Where(x => !dlcs.Contains(x.Key)).SelectMany(x => x.Value));
            exclusions.AddRange(_linkDiscMusic.Where(x => !x.Key.Invoke(dlcs)).Select(x => x.Value));

            var trackQuery = from track in allTrack
                             where categories.Contains(track.Category)
                             where !exclusions.Contains(track.Title)
                             select track;

            return trackQuery.ToList();
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
}
