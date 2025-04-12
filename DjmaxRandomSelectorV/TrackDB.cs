using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using Dmrsv.RandomSelector.Enums;

namespace DjmaxRandomSelectorV
{
    public class TrackDB
    {
        private const string AllTrackDownloadUrl = "https://v-archive.net/db/songs.json";
        private const string AllTrackDownloadUrlAlt = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/DMRSV3_Data/AllTrackList.json";
        private const string AllTrackFilePath = @"DMRSV3_Data\AllTrackList.json";

        private readonly IEventAggregator _eventAggregator;

        private readonly string[] _basicCategories;
        private readonly LinkDiscItem[] _linkDisc;

        public IReadOnlyList<Track> AllTrack { get; private set; }
        public IReadOnlyList<Track> Playable { get; private set; }
        
        public TrackDB(Dmrsv3AppData appdata)
        {
            _basicCategories = appdata.BasicCategories;
            _linkDisc = appdata.LinkDisc;
        }

        public void RequestDB(bool useAltDB = false)
        {
            using var client = new HttpClient();
            string url = useAltDB ? AllTrackDownloadUrlAlt : AllTrackDownloadUrl;
            string result = client.GetStringAsync(url).Result;

            using var writer = new StreamWriter(AllTrackFilePath);
            writer.Write(result);
        }

        public void ImportDB()
        {
            using var reader = new StreamReader(AllTrackFilePath);
            string json = reader.ReadToEnd();
            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var db = JsonSerializer.Deserialize<List<VArchiveDBTrack>>(json, option);

            AllTrack = db.ConvertAll(x =>
            {
                var info = new MusicInfo()
                {
                    Id = x.Title,
                    Title = x.Name,
                    Composer = x.Composer,
                    Category = x.DlcCode
                };
                return new Track()
                {
                    Info = info,
                    Patterns = x.Patterns
                                .SelectMany(bt => bt.Value, (bt, df) => new Pattern()
                                {
                                    Info = info,
                                    Button = bt.Key.AsButtonTunes(),
                                    Difficulty = df.Key.AsDifficulty(),
                                    Level = df.Value.Level
                                })
                                .OrderBy(p => p.PatternId)
                                .ToArray()
                };
            }).AsReadOnly();
        }

        public void SetPlayable(List<string> ownedDlcs)
        {
            var categories = ownedDlcs.Concat(_basicCategories);
            var exclusions = _linkDisc.Where(x => !x.RequiredDlc.Any(dlcs => dlcs.All(dlc => ownedDlcs.Contains(dlc))))
                                      .Select(x => x.Id);
            var playable = from track in AllTrack
                           where categories.Contains(track.Category)
                           where !exclusions.Contains(track.Id)
                           select track;
            Playable = playable.ToList().AsReadOnly();
        }

        public record VArchiveDBTrack
        {
            public int Title { get; init; }
            public string Name { get; init; }
            public string Composer { get; init; }
            public string DlcCode { get; init; }
            public string Dlc { get; init; }
            public Dictionary<string, Dictionary<string, VArchiveDBPattern>> Patterns { get; init; }
        }

        public record VArchiveDBPattern
        {
            public int Level { get; init; }
            public double Floor { get; init; }
            public int Rating { get; init; }
        }
    }
}
