using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace DjmaxRandomSelectorV
{
    public class VersionEventArgs : EventArgs
    {
        public string Version { get; }

        public VersionEventArgs(string version)
        {
            Version = version;
        }
    }
    public class VersionContainer
    {
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";

        public delegate void NewVersionAvailableEventHandler(object sender, VersionEventArgs e);
        public event NewVersionAvailableEventHandler NewAppVersionAvailable;
        public event NewVersionAvailableEventHandler NewAllTrackVersionAvailable;

        public Version CurrentAppVersion { get; private set; }
        public Version LastestAppVersion { get; private set; }
        public int AllTrackVersion { get; private set; }

        public VersionContainer(Version appVersion, int allTrackVersion)
        {
            CurrentAppVersion = appVersion;
            LastestAppVersion = appVersion;
            AllTrackVersion = allTrackVersion;
        }

        public async Task CheckLastestVersionsAsync()
        {
            using var client = new HttpClient();

            string result;
            try
            {
                result = await client.GetStringAsync(VersionsUrl);
            }
            catch
            {
                throw new Exception("Failed to check lastest versions.");
            }

            string[] versions = result.Split(',');
            LastestAppVersion = new Version(versions[0]);
            
            if (CurrentAppVersion < LastestAppVersion)
            {
                NewAppVersionAvailable?.Invoke(this, new VersionEventArgs(LastestAppVersion.ToString(3)));
            }

            int latestAllTrackVersion = Int32.Parse(versions[1]);
            if (AllTrackVersion != latestAllTrackVersion)
            {
                NewAllTrackVersionAvailable?.Invoke(this, new VersionEventArgs(versions[1]));
                AllTrackVersion = latestAllTrackVersion;
            }
        }
    }
}
