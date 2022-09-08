//#define debug

using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Effects;
using System.Net.Http;
using CsvHelper;
using System.Globalization;
using System.Linq;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private const int ApplicationVersion = 150;
        private const string DjmaxTitle = "DJMAX RESPECT V";
        
        private const string ReleasesUrl = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";

        private const string ConfigPath = "Data/Config.json";
        private const string AllTrackListPath = "Data/AllTrackList.csv";

        public ConditionalFilterViewModel ConditionalFilterViewModel { get; set; }
        public SelectiveFilterViewModel SelectiveFilterViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }
        public FilterOptionViewModel FilterOptionViewModel { get; set; }
        public FilterOptionIndicatorViewModel FilterOptionIndicatorViewModel { get; set; }

        private InfoViewModel _infoViewModel;

        #region Initializing
        private (int, int) CompareToLastestVersions(int allTrackVersion)
        {
            using var client = new HttpClient();

            string result = client.GetStringAsync(VersionsUrl).Result;
            string[] versions = result.Split(',');

            var lastestAppVersion = int.Parse(versions[0]);
            var lastestTrackVersion = int.Parse(versions[1]);

            int gapWithLastestApp = lastestAppVersion - ApplicationVersion;
            int gapWithLastestTrack = lastestTrackVersion - allTrackVersion;

            return (gapWithLastestApp, gapWithLastestTrack);
        }
        private void Initialize()
        {
            Config config = FileManager.Import<Config>(ConfigPath);
            // Check if the files should be updated, and then create Info dialog.
            try
            {
                (int gapWithLastestApp, int gapWithLastestTrack) = CompareToLastestVersions(config.AllTrackVersion);
                OpenReleasePageVisibility = gapWithLastestApp > 0 ? Visibility.Visible : Visibility.Hidden;
                if (gapWithLastestTrack != 0 || !File.Exists(AllTrackListPath))
                {
                    //DownloadAllTrackList();
                    config.AllTrackVersion += gapWithLastestTrack;
                    FileManager.Export(config, ConfigPath);
                    MessageBox.Show($"All track list is updated to the version {config.AllTrackVersion}.",
                        "DJMAX Random Selector V",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                _infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion + gapWithLastestApp,
                    config.AllTrackVersion);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Cannot check available updates. Check the internet connection.",
                    "Selector Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                _infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion,
                    config.AllTrackVersion);
            }
        }
        #endregion

        private IEventAggregator _eventAggregator;
        public MainViewModel()
        {
            _eventAggregator = new EventAggregator();

            ConditionalFilterViewModel = new ConditionalFilterViewModel();
            SelectiveFilterViewModel = new SelectiveFilterViewModel();
            HistoryViewModel = new HistoryViewModel();
            FilterOptionViewModel = new FilterOptionViewModel();

            FilterOptionIndicatorViewModel = new FilterOptionIndicatorViewModel();
        }



        #region On Start Up
        public void SetPosition(double[] position)
        {
            Window window = Application.Current.MainWindow;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
            else
            {
                position = new double[2] { window.Top, window.Left };
            }
        }
        #endregion

        //#region On Exit
        //public void SaveConfig(object view)
        //{
        //    var window = view as Window;

        //    FileManager.Export(_filter, CurrentFilterPath);
        //    FileManager.Export(_playlist, CurrentPlaylistPath);

        //    if (!_config.SavesRecents)
        //    {
        //        _filter.Recents.Clear();
        //        _playlist.Recents.Clear();
        //    }
        //    _config.Position = new double[2] { window.Top, window.Left };
        //    FileManager.Export(_config, ConfigPath);
        //}
        //#endregion

        private bool _isFilterType = true;
        private bool _isPlaylistType = false;
        public bool IsFilterType
        {
            get { return _isFilterType; }
            set
            {
                _isFilterType = value;
                NotifyOfPropertyChange(() => IsFilterType);
            }
        }
        public bool IsPlaylistType
        {
            get { return _isPlaylistType; }
            set
            {
                _isPlaylistType = value;
                NotifyOfPropertyChange(() => IsPlaylistType);
            }
        }
        public void ChangeTypeOfFilter(bool isPlaylist)
        {
            IsFilterType = !isPlaylist;
            IsPlaylistType = isPlaylist;
        }

        #region Window Top Bar
        private Visibility _openReleasePageVisibility;
        public Visibility OpenReleasePageVisibility
        {
            get { return _openReleasePageVisibility; }
            set
            {
                _openReleasePageVisibility = value;
                NotifyOfPropertyChange(() => OpenReleasePageVisibility);
            }
        }

        public void OpenReleasePage()
        {
            System.Diagnostics.Process.Start(ReleasesUrl);
        }
        public void MoveWindow(object view)
        {
            var window = view as Window;
            window.DragMove();
        }
        public void MinimizeWindow(object view)
        {
            var window = view as Window;
            window.WindowState = WindowState.Minimized;
        }
        public void CloseWindow(object view)
        {
            var window = view as Window;
            window.Close();
        }
        #endregion

        #region Tab Bar
        private string _currentTab = "FILTER";
        private bool _isFilterTabSelected = true;
        private bool _isHistoryTabSelected = false;
        public string CurrentTab
        {
            get { return _currentTab; }
            set 
            { 
                _currentTab = value; 
                NotifyOfPropertyChange(() => CurrentTab);
            }
        }
        public bool IsFilterTabSelected
        {
            get { return _isFilterTabSelected; }
            set 
            {
                _isFilterTabSelected = value;
                NotifyOfPropertyChange(() => IsFilterTabSelected);
            }
        }
        public bool IsHistoryTabSelected
        {
            get { return _isHistoryTabSelected; }
            set
            {
                _isHistoryTabSelected = value;
                NotifyOfPropertyChange(() => IsHistoryTabSelected);
            }
        }
        public void LoadFilterTab()
        {
            IsFilterTabSelected = true;
            CurrentTab = "FILTER";
        }
        public void LoadHistoryTab()
        {
            IsHistoryTabSelected = true;
            CurrentTab = "HISTORY";
        }
        #endregion

        #region Another Windows
        private readonly IWindowManager _windowManager = new WindowManager();
        public void ShowInfo()
        {
            _windowManager.ShowDialogAsync(_infoViewModel);
        }
        public void ShowSetting()
        {
            _windowManager.ShowDialogAsync(new SettingViewModel());
        }
        #endregion
    }
}
