using Caliburn.Micro;
using DjmaxRandomSelectorV.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class InfoViewModel : Screen
    {
        public InfoViewModel(int selectorVersion)
        {
            var str = selectorVersion.ToString();
            for (int i = 1; i <= 5; i += 2)
            {
                str = str.Insert(i, ".");
            }
            CurrentVersion = str;
            AllTrackVersion = Settings.Default.allTrackVersion.ToString();
        }

        private string _currentVersion;
        private string _allTrackVersion;
        public string CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                _currentVersion = $"Current Version: {value}";
                NotifyOfPropertyChange(() => CurrentVersion);
            }
        }
        public string AllTrackVersion
        {
            get { return _allTrackVersion; }
            set
            {
                _allTrackVersion = $"All Track Version: {value}";
                NotifyOfPropertyChange(() => AllTrackVersion);
            }
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
