using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV
{
    public class UpdateManager
    {
        // TODO: restore url to main
        private const string VersionCheckUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/refs/heads/develop-ver3/DjmaxRandomSelectorV/Version3.txt";
        private const string AllTrackDownloadUrl = "https://v-archive.net/db/songs.json";
        private const string AppdataDownloadUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/refs/heads/develop-ver3/DjmaxRandomSelectorV/DMRSV3_Data/appdata.json";
        private const string AllTrackFilePath = @"DMRSV3_Data\AllTrackList.json";
        private const string AppdataFilePath = @"DMRSV3_Data\appdata.json";

        private readonly VersionContainer _container;
        private readonly IFileManager _fileManager;

        public UpdateManager(Dmrsv3Configuration config, VersionContainer container, IFileManager fileManager)
        {
            _container = container;
            _fileManager = fileManager;
            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            _container.CurrentAppVersion = _container.LatestAppVersion = assemblyVersion;
            var versionInfo = config.VersionInfo;
            _container.AllTrackVersion = versionInfo.AllTrackVersion;
            _container.AppdataVersion = versionInfo.AppdataVersion;
        }

        public async Task UpdateAsync()
        {
            string[] versions; // [ app version, appdata version, notice header, notice body ]
            try
            {
                string result = await _fileManager.RequestAsync(VersionCheckUrl);
                versions = result.Split('\n');
            }
            catch
            {
                throw new Exception("Failed to check update.");
            }
            _container.LatestAppVersion = new Version(versions[0]);

            var tasks = new List<Task<int>>();
            // update all track
            DateTime now = DateTime.Now, past = DateTime.MinValue;
            if (!File.Exists(AppdataFilePath)
                || !DateTime.TryParse(_container.AllTrackVersion, out past)
                || now.CompareTo(past) > 0)
            {
                tasks.Add(DownloadAllTrackAsync());
            }
            // update appdata
            if (versions[1].CompareTo(_container.AppdataVersion) > 0)
            {
                tasks.Add(DownloadAppdataAsync());
            }

            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                int result = await finishedTask;
                switch (result)
                {
                    case 0:
                        _container.AllTrackVersion = now.ToString("g");
                        break;
                    case 1:
                        _container.AppdataVersion = versions[1];
                        break;
                }
                tasks.Remove(finishedTask);
            }
        }

        private async Task<int> DownloadAllTrackAsync()
        {
            try
            {
                string result = await _fileManager.RequestAsync(AllTrackDownloadUrl);
                _fileManager.Write(result, AllTrackFilePath);
            }
            catch
            {
                return -1;
            }
            return 0;
        }

        private async Task<int> DownloadAppdataAsync()
        {
            try
            {
                string result = await _fileManager.RequestAsync(AppdataDownloadUrl);
                _fileManager.Write(result, AppdataFilePath);
            }
            catch
            {
                return -1;
            }
            return 1;
        }
    }
}
