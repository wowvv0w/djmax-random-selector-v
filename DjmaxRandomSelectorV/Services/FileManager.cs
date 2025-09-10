using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.SerializableObjects.Deprecated;
using DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible;
using Microsoft.Win32;

namespace DjmaxRandomSelectorV.Services
{
    public class FileManager : IFileManager
    {
        #region Paths
        private const string VersionCheckUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version3.txt";

        private readonly Dictionary<Type, string> _filePathMap = new()
        {
            [typeof(Dmrsv3Appdata)] = @"DMRSV3_Data\appdata.json",
            [typeof(Dmrsv3Configuration)] = @"DMRSV3_Data\Config.json",
            [typeof(Dmrsv3BasicFilterPreset)] = @"DMRSV3_Data\CurrentFilter.json",
            [typeof(Dmrsv2PlaylistFilterPreset)] = @"DMRSV3_Data\CurrentPlaylist.json",
            [typeof(VArchiveDBRoot)] = @"DMRSV3_Data\AllTrackList.json",
        };

        private readonly Dictionary<Type, string> _dirPathMap = new()
        {
            [typeof(Dmrsv3BasicFilterPreset)] = @"DMRSV3_Data\Preset",
            [typeof(Dmrsv2PlaylistFilterPreset)] = @"DMRSV3_Data\Playlist",
        };

        private readonly Dictionary<Type, string> _downloadUrlMap = new()
        {
            [typeof(Dmrsv3Appdata)] = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/DMRSV3_Data/appdata.json",
            [typeof(VArchiveDBRoot)] = "https://v-archive.net/db/songs.json",
        };
        #endregion

        public T Load<T>(bool useDialog = false)
        {
            string path = _filePathMap[typeof(T)];
            if (useDialog)
            {
                path = ShowOpenFileDialog(_dirPathMap[typeof(T)]) ?? path;
            }

            using var reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            T instance = JsonSerializer.Deserialize<T>(json, option);
            return instance ?? throw new NullReferenceException();
        }

        public void Save<T>(T instance, bool useDialog = false)
        {
            string path = _filePathMap[typeof(T)];
            if (useDialog)
            {
                path = ShowSaveFileDialog(_dirPathMap[typeof(T)]) ?? path;
            }

            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            string json = JsonSerializer.Serialize(instance, option);
            using var writer = new StreamWriter(path);
            writer.Write(json);
        }

        public async Task DownloadAsync<T>()
        {
            string url = _downloadUrlMap[typeof(T)];
            string path = _filePathMap[typeof(T)];

            using var client = new HttpClient();
            string result = await client.GetStringAsync(url);
            using var writer = new StreamWriter(path);
            writer.Write(result);
        }

        public async Task<string[]> CheckUpdatesAsync()
        {
            using var client = new HttpClient();
            string result = await client.GetStringAsync(VersionCheckUrl);
            return result.Split('\n');
        }

        public bool Exists<T>()
        {
            string path = _filePathMap[typeof(T)];
            return File.Exists(path);
        }

        #region Helper Functions
        private string ShowSaveFileDialog(string initialDir)
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, initialDir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var dialog = new SaveFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };
            bool? result = dialog.ShowDialog();
            return result == true ? dialog.FileName : null;
        }

        private string ShowOpenFileDialog(string initialDir)
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, initialDir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };
            bool? result = dialog.ShowDialog();
            return result == true ? dialog.FileName : null;
        }
        #endregion
    }
}
