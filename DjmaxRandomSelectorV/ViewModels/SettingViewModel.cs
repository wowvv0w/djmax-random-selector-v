using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;
using Microsoft.Win32;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SettingViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingStateManager _settingManager;
        private readonly ISettingState _setting;
        private readonly IEnumerable<Dmrsv3Category> _categories;

        public object IsPlaylistUpdater { get; }
        public object InputDelayUpdater { get; }
        public object SavesRecentsUpdater { get; }
        public BindableCollection<object> CategoryUpdaters { get; }

        public SettingViewModel(IEventAggregator eventAggregator, ISettingStateManager settingManager, ITrackDB trackDB)
        {
            _eventAggregator = eventAggregator;
            _settingManager = settingManager;
            _setting = _settingManager.GetSetting();

            IsPlaylistUpdater = new SettingToggleItem(
                "PLAYLIST MODE",
                () => _setting.FilterType == FilterType.Playlist,
                newValue => _setting.FilterType = newValue ? FilterType.Playlist : FilterType.Query);

            InputDelayUpdater = new SettingSliderItem(
                "INPUT DELAY",
                10,
                50,
                5,
                () => _setting.InputDelay,
                newValue => _setting.InputDelay = newValue,
                value => $"{value}ms");

            SavesRecentsUpdater = new SettingToggleItem(
                "SAVE RECENT MUSIC LIST",
                () => _setting.SavesRecents,
                newValue => _setting.SavesRecents = newValue);

            _categories = trackDB.Categories.Where(cat => !(string.IsNullOrEmpty(cat.SteamId) && cat.Type != 3)); // TODO: use enum
            CategoryUpdaters = new BindableCollection<object>(_categories.Select(cat =>
            {
               return new SettingToggleItem(
                   cat.Name,
                   () => _setting.OwnedDlcs.Contains(cat.Id),
                   newValue =>
                   {
                       if (newValue)
                       {
                           _setting.OwnedDlcs.Add(cat.Id);
                       }
                       else
                       {
                           _setting.OwnedDlcs.Remove(cat.Id);
                       }
                   });
            }));
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
