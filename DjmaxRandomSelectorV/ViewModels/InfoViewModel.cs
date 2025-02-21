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

        public InfoViewModel()
        {
            var updater = IoC.Get<UpdateManager>();
            CurrentVersion = "Current Version: " + updater.CurrentAppVersion.ToString(3);
            LastestVersion = "Lastest Version: " + updater.LatestAppVersion.ToString(3);
            AllTrackVersion = "All Track Version : " + updater.AllTrackVersion.ToString();
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
