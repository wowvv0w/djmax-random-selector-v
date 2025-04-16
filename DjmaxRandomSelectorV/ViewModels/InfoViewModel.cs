using Caliburn.Micro;
using System.Diagnostics;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class InfoViewModel : Screen
    {
        private const string GITHUB_PAGE_URL = "https://github.com/wowvv0w/djmax-random-selector-v";
        private const string BUG_REPORT_URL = "https://github.com/wowvv0w/djmax-random-selector-v/issues";

        public string CurrentVersion { get; }
        public string LastestVersion { get; }
        public string AllTrackVersion { get; }
        public string AppdataVersion { get; }

        public InfoViewModel()
        {
            var container = IoC.Get<VersionContainer>();
            CurrentVersion = "Current Version: " + container.CurrentAppVersion.ToString(3);
            LastestVersion = "Lastest Version: " + container.LatestAppVersion.ToString(3);
            AllTrackVersion = "All Track Last Updated : " + container.AllTrackVersion;
            AppdataVersion = "Appdata Version : " + container.AppdataVersion;
        }

        public void OpenGithubPage()
        {
            Process.Start("explorer.exe", GITHUB_PAGE_URL);
        }

        public void OpenBugReport()
        {
            Process.Start("explorer.exe", BUG_REPORT_URL);
        }

        public void CloseDialog()
        {
            TryCloseAsync();
        }
    }
}
