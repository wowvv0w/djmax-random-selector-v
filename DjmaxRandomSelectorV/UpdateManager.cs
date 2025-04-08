using System;
using System.Net.Http;

namespace DjmaxRandomSelectorV
{
    public class UpdateManager
    {
        private const string VersionCheckUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/refs/heads/feature-new-db/DjmaxRandomSelectorV/Version3.txt";
        
        public Version CurrentAppVersion { get; private set; }
        public Version LatestAppVersion { get; private set; }
        public int AllTrackVersion { get; private set; }

        public UpdateManager(Dmrsv3Configuration config, Version appVer)
        {
            CurrentAppVersion = appVer;
            LatestAppVersion = appVer;
            AllTrackVersion = config.VersionInfo.AllTrackVersion;
        }

        public bool[] CheckUpdates()
        {
            using var client = new HttpClient();

            string result;
            try
            {
                result = client.GetStringAsync(VersionCheckUrl).Result;
            }
            catch
            {
                throw new Exception("Failed to check lastest versions.");
            }

            string[] versions = result.Split('\n');
            LatestAppVersion = new Version(versions[0]);
            int latestAllTrackVersion = int.Parse(versions[1]);
            bool allTrackUpdateAvailable = AllTrackVersion != latestAllTrackVersion;
            AllTrackVersion = latestAllTrackVersion;
            return new bool[] { CurrentAppVersion < LatestAppVersion, allTrackUpdateAvailable };
        }
    }
}
