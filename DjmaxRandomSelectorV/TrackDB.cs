using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV
{
    public class TrackDB : IHandle<SettingMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly string _allTrackDownloadUrl;
        private readonly string _allTrackDownloadUrlAlt;
        private readonly string _allTrackFilePath;
        private readonly string[] _basicCategories;
        private readonly (int Id, string[][] RequiredDlc)[] _linkDisc;

        public IEnumerable<Track> AllTrack { get; private set; }
        public IEnumerable<Track> Playable { get; private set; }
        
        public TrackDB(Dmrsv3CoreAppData appdata, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            
            _allTrackDownloadUrl = appdata.AllTrackDownloadUrl;
            _allTrackDownloadUrlAlt = appdata.AllTrackDownloadUrlAlt;
            _allTrackFilePath = $@"{appdata.DataFolderName}\{appdata.AllTrackFileName}";
            _basicCategories = appdata.BasicCategories;
            _linkDisc = appdata.LinkDisc;
        }

        public void RequestDB(bool useAltDB = false)
        {
            using var client = new HttpClient();
            string url = useAltDB ? _allTrackDownloadUrlAlt : _allTrackDownloadUrl;
            string result = client.GetStringAsync(url).Result;

            using var writer = new StreamWriter(_allTrackFilePath);
            writer.Write(result);
        }

        public void ImportDB()
        {
            using var reader = new StreamReader(_allTrackFilePath);
            string json = reader.ReadToEnd();
            var db = JsonSerializer.Deserialize<List<VArchiveDBTrack>>(json);

            AllTrack = db.ConvertAll(x => new Track()
            {
                Id = x.Title,
                Title = x.Name,
                Composer = x.Composer,
                Category = x.DlcCode,
                PatternLevelTable = x.Patterns
                                     .SelectMany(btList => btList.Value,
                                                 (btList, dfList) => new { style = $"{btList.Key}{dfList.Key}", pattern = dfList.Value })
                                     .Where(table => table.pattern is not null)
                                     .Select(table => new KeyValuePair<string, int>(table.style, table.pattern.Level))
                                     .ToDictionary(pair => pair.Key, pair => pair.Value)
            });
        }

        private void SetPlayable(List<string> ownedDlcs)
        {
            var categories = ownedDlcs.Concat(_basicCategories);
            var exclusions = _linkDisc.Where(x => x.RequiredDlc.Any(dlcs => dlcs.All(dlc => ownedDlcs.Contains(dlc))))
                                      .Select(x => x.Id);
            var playable = from track in AllTrack
                           where categories.Contains(track.Category)
                           where !exclusions.Contains(track.Id)
                           select track;
            Playable = playable;
        }

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            SetPlayable(message.OwnedDlcs);
            return Task.CompletedTask;
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
