using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Services
{
    public class UpdateManager
    {
        private const string VersionCheckUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version3.txt";
        private const string AllTrackDownloadUrl = "https://v-archive.net/db/songs.json";
        private const string AppdataDownloadUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/DMRSV3_Data/appdata.json";
        private const string AllTrackFilePath = @"DMRSV3_Data\AllTrackList.json";
        private const string AppdataFilePath = @"DMRSV3_Data\appdata.json";

        private readonly IFileManager _fileManager;
        private readonly IVersionInfoStateManager _versionInfoManager;

        public UpdateManager(IFileManager fileManager, IVersionInfoStateManager versionInfoManager)
        {
            _fileManager = fileManager;
            _versionInfoManager = versionInfoManager;
        }

        public async Task UpdateAsync()
        {
            string[] versions; // [ latest app version, latest appdata version, notice header, notice body ]
            try
            {
                string result = await _fileManager.RequestAsync(VersionCheckUrl);
                versions = result.Split('\n');
            }
            catch
            {
                throw new Exception("Failed to check update.");
            }
            var versionInfo = _versionInfoManager.GetVersionInfo();
            versionInfo.LatestAppVersion = new Version(versions[0]);

            var tasks = new List<Task<int>>();
            // update all track
            long now = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            long past = versionInfo.AllTrackVersion;
            if (now > past || !File.Exists(AllTrackFilePath))
            {
                Debug.WriteLine("all track update start");
                tasks.Add(DownloadAllTrackAsync());
            }
            // update appdata
            if (!File.Exists(AppdataFilePath)
                || versions[1].CompareTo(versionInfo.AppdataVersion) > 0)
            {
                Debug.WriteLine("appdata update start");
                tasks.Add(DownloadAppdataAsync());
            }

            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                int result = await finishedTask;
                switch (result)
                {
                    case 0:
                        versionInfo.AllTrackVersion = now;
                        break;
                    case 1:
                        versionInfo.AppdataVersion = versions[1];
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
