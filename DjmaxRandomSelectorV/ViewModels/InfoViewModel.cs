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
            var vc = IoC.Get<VersionContainer>();
            CurrentVersion = "Current Version: " + IntToString(vc.CurrentAppVersion);
            LastestVersion = "Lastest Version: " + IntToString(vc.LastestAppVersion);
            AllTrackVersion = "All Track Version : " + vc.AllTrackVersion.ToString();

            string IntToString(int version)
            {
                var str = version.ToString();
                for (int i = 1; i <= 3; i += 2)
                {
                    str = str.Insert(i, ".");
                }
                return str;
            }
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
