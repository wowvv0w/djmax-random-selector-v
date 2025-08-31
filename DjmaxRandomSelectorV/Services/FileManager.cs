using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DjmaxRandomSelectorV.RandomSelector;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.SerializableObjects.Deprecated;
using DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible;

namespace DjmaxRandomSelectorV.Services
{
    public class FileManager : IUpdateDownloader
    {
        #region Paths
        private const string AppdataFilePath = @"DMRSV3_Data\appdata.json";
        private const string ConfigFilePath = @"DMRSV3_Data\Config.json";
        private const string AllTrackFilePath = @"DMRSV3_Data\AllTrackList.json";
        private const string CurrentBasicFilterFilePath = @"DMRSV3_Data\CurrentFilter.json";
        private const string BasicFilterPresetDirPath = @"DMRSV3_Data\Preset";
        private const string CurrentAdvancedFilterFilePath = @"DMRSV3_Data\CurrentPlaylist.json";
        private const string AdvancedFilterPresetDirPath = @"DMRSV3_Data\Playlist";
        private const string VersionCheckUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version3.txt";
        private const string AllTrackDownloadUrl = "https://v-archive.net/db/songs.json";
        private const string AppdataDownloadUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/DMRSV3_Data/appdata.json";
        #endregion


        #region Configuration
        public Dmrsv3Configuration LoadConfig()
        {
            try
            {
                return Import<Dmrsv3Configuration>(ConfigFilePath);
            }
            catch
            {
                return new Dmrsv3Configuration();
            }
        }
        
        public void SaveConfig(Dmrsv3Configuration config)
        {
            Export(config, ConfigFilePath);
        }
        #endregion

        #region Appdata
        public Dmrsv3Appdata LoadAppdata()
        {
            return Import<Dmrsv3Appdata>(AppdataFilePath);
        }

        public bool ExistsAppdataFile()
        {
            return File.Exists(AppdataFilePath);
        }

        public async Task DownloadAppdataAsync()
        {
            await RequestAsync(AppdataDownloadUrl, AppdataFilePath);
        }
        #endregion

        #region All Track
        public Dictionary<int, Track> LoadAllTrack()
        {
            var db = Import<VArchiveDBRoot>(AllTrackFilePath);
            return db.Select(x =>
            {
                var info = new MusicInfo()
                {
                    Title = x.Name,
                    Composer = x.Composer,
                    Category = x.DlcCode
                };
                return new Track()
                {
                    Id = x.Title,
                    Info = info,
                    Patterns = x.Patterns
                        .SelectMany(bt => bt.Value, (bt, df) => new Pattern()
                        {
                            Id = new PatternId(x.Title, bt.Key.AsButtonTunes(), df.Key.AsDifficulty()),
                            Info = info,
                            Level = df.Value.Level
                        })
                        .OrderBy(p => p.Id)
                        .ToArray(),
                    UserTags = TrackUserTags.None
                };
            }).ToDictionary(t => t.Id);
        }

        public bool ExistsAllTrackFile()
        {
            return File.Exists(AllTrackFilePath);
        }

        public async Task DownloadAllTrackAsync()
        {
            await RequestAsync(AllTrackDownloadUrl, AllTrackFilePath);
        }
        #endregion

        #region Update Check
        public async Task<string[]> CheckUpdatesAsync()
        {
            using var client = new HttpClient();
            string result = await client.GetStringAsync(VersionCheckUrl);
            return result.Split('\n');
        }
        #endregion

        #region Helper Functions
        private T Import<T>(string path)
        {
            using var reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            T instance = JsonSerializer.Deserialize<T>(json, option);
            return instance ?? throw new NullReferenceException();
        }

        private void Export<T>(T instance, string path)
        {
            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            string json = JsonSerializer.Serialize(instance, option);
            using var writer = new StreamWriter(path);
            writer.Write(json);
        }

        public async Task RequestAsync(string sourceUrl, string destinationPath)
        {
            using var client = new HttpClient();
            string result = await client.GetStringAsync(sourceUrl);
            using var writer = new StreamWriter(destinationPath);
            writer.Write(result);
        }
        #endregion
    }
}
