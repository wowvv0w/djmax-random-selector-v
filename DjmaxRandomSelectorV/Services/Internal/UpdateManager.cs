using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible;

namespace DjmaxRandomSelectorV.Services.Internal
{
    public class UpdateManager
    {
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
                versions = await _fileManager.CheckUpdatesAsync();
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
            if (now > past || !_fileManager.Exists<VArchiveDBRoot>())
            {
                System.Diagnostics.Debug.WriteLine("all track update start");
                tasks.Add(
                    _fileManager
                    .DownloadAsync<VArchiveDBRoot>()
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            versionInfo.AllTrackVersion = now;
                        }
                    }));
            }

            // update appdata
            if (!_fileManager.Exists<Dmrsv3Appdata>()
                || versions[1].CompareTo(versionInfo.AppdataVersion) > 0)
            {
                System.Diagnostics.Debug.WriteLine("appdata update start");
                tasks.Add(
                    _fileManager
                    .DownloadAsync<Dmrsv3Appdata>()
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
