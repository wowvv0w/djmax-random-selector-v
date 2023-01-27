using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SettingViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IFileManager _fileManager;
        private readonly SettingMessage _message;
        private readonly List<Category> _categories;

        public bool IsPlaylist
        {
            get { return _message.FilterType == FilterType.Playlist; }
            set
            {
                _message.FilterType = value ? FilterType.Playlist : FilterType.Query;
                NotifyOfPropertyChange();
            }
        }
        public int InputDelay
        {
            get { return _message.InputInterval; }
            set
            {
                _message.InputInterval = value;
                NotifyOfPropertyChange();
            }
        }
        public bool SavesRecents
        {
            get { return _message.SavesExclusion; }
            set
            {
                _message.SavesExclusion = value;
                NotifyOfPropertyChange();
            }
        }
        public BindableCollection<ListUpdater> CategoryUpdaters { get; }

        public SettingViewModel(IEventAggregator eventAggregator, IFileManager fileManager)
        {
            _eventAggregator = eventAggregator;
            _fileManager = fileManager;

            var config = IoC.Get<Configuration>();
            _message = new SettingMessage()
            {
                FilterType = config.FilterType,
                InputInterval = config.InputDelay,
                SavesExclusion = config.SavesRecents,
                OwnedDlcs = config.OwnedDlcs.ConvertAll(x => x)
            };

            _categories = IoC.Get<CategoryContainer>().GetCategories();
            _categories.RemoveAll(x => string.IsNullOrEmpty(x.Code));
            var updaters = _categories.ConvertAll(x => new ListUpdater(x.Name, x.Id, _message.OwnedDlcs));
            CategoryUpdaters = new BindableCollection<ListUpdater>(updaters);
        }

        public void DetectDlcs()
        {
            Dictionary<string, string> dlcCodes = _categories.ToDictionary(x => x.Code, x => x.Id);

            var ownedDlcs = _message.OwnedDlcs;
            ownedDlcs.Clear();

            string steamKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam";
            string steamPath = Registry.GetValue(steamKeyName, "InstallPath", null).ToString();
            
            var libraryPath = new DirectoryInfo($"{steamPath}\\appcache\\librarycache");

            foreach (FileInfo file in libraryPath.GetFiles())
            {
                if (file.Extension.ToLower().Equals(".jpg"))
                {
                    string dlc = dlcCodes.GetValueOrDefault(file.Name[..^11], null);
                    if (!string.IsNullOrEmpty(dlc))
                    {
                        ownedDlcs.Add(dlc);
                    }
                }
            }

            CategoryUpdaters.Refresh();
            MessageBox.Show($"{ownedDlcs.Count} DLCs are detected.",
                "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Apply()
        {
            var config = IoC.Get<Configuration>();
            config.FilterType = _message.FilterType;
            config.InputDelay = _message.InputInterval;
            config.SavesRecents = _message.SavesExclusion;
            config.OwnedDlcs = _message.OwnedDlcs.ConvertAll(x => x);

            _fileManager.Export(config, @"Data\Config.json");
            _eventAggregator.PublishOnUIThreadAsync(_message);
            TryCloseAsync(true);
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }
    }
}
