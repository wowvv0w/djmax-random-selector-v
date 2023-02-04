using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace DjmaxRandomSelectorV
{
    public class VersionEventArgs : EventArgs
    {
        public int Version { get; }

        public VersionEventArgs(int version)
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

        public int CurrentAppVersion { get; private set; }
        public int LastestAppVersion { get; private set; }
        public int AllTrackVersion { get; private set; }

        public VersionContainer(int appVersion, int allTrackVersion)
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

            int[] versions = Array.ConvertAll(result.Split(','), int.Parse);

            if (CurrentAppVersion < versions[0])
            {
                NewAppVersionAvailable?.Invoke(this, new VersionEventArgs(versions[0]));
            }
            LastestAppVersion = versions[0];
            if (AllTrackVersion != versions[1])
            {
                NewAllTrackVersionAvailable?.Invoke(this, new VersionEventArgs(versions[1]));
                AllTrackVersion = versions[1];
            }
        }
    }
}
