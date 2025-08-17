using System;
using System.Diagnostics;
using System.Globalization;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Services;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class InfoViewModel : Screen
    {
        private const string GITHUB_PAGE_URL = "https://github.com/wowvv0w/djmax-random-selector-v";
        private const string BUG_REPORT_URL = "https://github.com/wowvv0w/djmax-random-selector-v/issues";

        public string CurrentVersion { get; }
        public string LatestVersion { get; }
        public string AllTrackVersion { get; }
        public string AppdataVersion { get; }

        public InfoViewModel(IReadOnlyVersionInfoStateManager versionInfoManager)
        {
            var versionInfo = versionInfoManager.GetReadOnlyVersionInfo();
            CurrentVersion = "Current Version: " + versionInfo.CurrentAppVersion.ToString(3);
            LatestVersion = "Latest Version: " + versionInfo.LatestAppVersion.ToString(3);
            AllTrackVersion = "All Track Last Updated : " + DateTime.ParseExact(versionInfo.AllTrackVersion.ToString(),
                                                                                "yyMMddHHmm",
                                                                                CultureInfo.InvariantCulture);
            AppdataVersion = "Appdata Version : " + versionInfo.AppdataVersion;
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
