using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Services
{
    public class UpdateManager
    {
        private readonly IUpdateDownloader _updateDownloader;
        private readonly IVersionInfoStateManager _versionInfoManager;

        public UpdateManager(IUpdateDownloader updateDownloader, IVersionInfoStateManager versionInfoManager)
        {
            _updateDownloader = updateDownloader;
            _versionInfoManager = versionInfoManager;
        }

        public async Task UpdateAsync()
        {
            string[] versions; // [ latest app version, latest appdata version, notice header, notice body ]
            try
            {
                versions = await _updateDownloader.CheckUpdatesAsync();
            }
            catch
            {
                throw new Exception("Failed to check update.");
            }
            var versionInfo = _versionInfoManager.GetVersionInfo();
            versionInfo.LatestAppVersion = new Version(versions[0]);

            var tasks = new List<Task>();
            // update all track
            long now = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            long past = versionInfo.AllTrackVersion;
            if (now > past || !_updateDownloader.ExistsAllTrackFile())
            {
                System.Diagnostics.Debug.WriteLine("all track update start");
                tasks.Add(
                    _updateDownloader
                    .DownloadAllTrackAsync()
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            versionInfo.AllTrackVersion = now;
                        }
                    }));
            }

            // update appdata
            if (!_updateDownloader.ExistsAppdataFile()
                || versions[1].CompareTo(versionInfo.AppdataVersion) > 0)
            {
                System.Diagnostics.Debug.WriteLine("appdata update start");
                tasks.Add(
                    _updateDownloader
                    .DownloadAppdataAsync()
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            versionInfo.AppdataVersion = versions[1];
                        }
                    }));
            }

            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
            }
        }
    }
}
