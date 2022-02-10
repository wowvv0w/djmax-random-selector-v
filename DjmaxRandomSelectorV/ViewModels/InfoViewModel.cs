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
        public InfoViewModel(int currentVersion, int lastVersion)
        {
            CurrentVersion = IntToString(currentVersion);
            LastVersion = IntToString(lastVersion);
            AllTrackVersion = Settings.Default.allTrackVersion.ToString();

            string IntToString(int version)
            {
                var str = version.ToString();
                for (int i = 1; i <= 5; i += 2)
                {
                    str = str.Insert(i, ".");
                }
                return str;
            }
        }

        private string _currentVersion;
        private string _lastVersion;
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
        public string LastVersion
        {
            get { return _lastVersion; }
            set
            {
                _lastVersion = $"Last Version: {value}";
                NotifyOfPropertyChange(() => LastVersion);
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
