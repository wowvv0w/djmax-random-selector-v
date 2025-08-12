using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;
using Dmrsv.RandomSelector;
using Microsoft.Win32;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SettingViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingStateManager _settingManager;
        private readonly ISettingState _setting;
        private readonly IEnumerable<Category> _categories;

        public bool IsPlaylist
        {
            get { return _setting.FilterType == FilterType.Playlist; }
            set
            {
                _setting.FilterType = value ? FilterType.Playlist : FilterType.Query;
                NotifyOfPropertyChange();
            }
        }
        public int InputDelay
        {
            get { return _setting.InputDelay; }
            set
            {
                _setting.InputDelay = value;
                NotifyOfPropertyChange();
            }
        }
        public bool SavesRecents
        {
            get { return _setting.SavesRecents; }
            set
            {
                _setting.SavesRecents = value;
                NotifyOfPropertyChange();
            }
        }
        public BindableCollection<ListUpdater> CategoryUpdaters { get; }

        public SettingViewModel(IEventAggregator eventAggregator, ISettingStateManager settingManager, ITrackDB trackDB)
        {
            _eventAggregator = eventAggregator;
            _settingManager = settingManager;

            var config = _settingManager.GetSetting();

            _categories = trackDB.Categories.Where(cat => !(string.IsNullOrEmpty(cat.SteamId) && cat.Type != 3)); // TODO: use enum
            var updaters = _categories.Select(cat => new ListUpdater(cat.Name, cat.Id, _setting.OwnedDlcs));
            CategoryUpdaters = new BindableCollection<ListUpdater>(updaters);
        }

        public void DetectDlcs()
        {
            Dictionary<string, string> dlcCodes = _categories.Where(cat => cat.SteamId is not null).ToDictionary(cat => cat.SteamId, cat => cat.Id);

            var ownedDlcs = _setting.OwnedDlcs;
            ownedDlcs.Clear();

            string steamKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam";
            string steamPath = Registry.GetValue(steamKeyName, "InstallPath", null).ToString();
            
            var libraryPath = new DirectoryInfo($"{steamPath}\\appcache\\librarycache");

            foreach (DirectoryInfo dir in libraryPath.GetDirectories())
            {
                string dlc = dlcCodes.GetValueOrDefault(dir.Name, null);
                if (!string.IsNullOrEmpty(dlc))
                {
                    ownedDlcs.Add(dlc);
                }
            }

            CategoryUpdaters.Refresh();
            MessageBox.Show($"{ownedDlcs.Count} DLCs are detected.",
                "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Apply()
        {
            _eventAggregator.PublishOnUIThreadAsync(new FilterTypeChangedMessage(_setting.FilterType));
            _settingManager.SetSetting(_setting);
            TryCloseAsync(true);
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }
    }
}
