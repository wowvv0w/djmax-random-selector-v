﻿using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using DjmaxRandomSelectorV.RandomSelector;
using System.Threading;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>, IHandle<SelectorOption>
    {
        private const int ApplicationVersion = 150;
        private const string DjmaxTitle = "DJMAX RESPECT V";
        
        private const string ReleasesUrl = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";

        private const string ConfigPath = "Data/Config.json";
        private const string AllTrackListPath = "Data/AllTrackList.csv";

        private FilterBaseViewModel _filterViewModel;
        public FilterBaseViewModel FilterViewModel
        {
            get { return _filterViewModel; }
            set
            {
                _filterViewModel = value;
                NotifyOfPropertyChange(() => FilterViewModel);
            }
        }
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
        private void Initialize(Config config)
        {
            // Check if the files should be updated, and then create Info dialog.
            try
            {
                (int gapWithLastestApp, int gapWithLastestTrack) = CompareToLastestVersions(config.AllTrackVersion);
                OpenReleasePageVisibility = gapWithLastestApp > 0 ? Visibility.Visible : Visibility.Hidden;
                if (gapWithLastestTrack != 0 || !File.Exists(AllTrackListPath))
                {
                    FileManager.DownloadAllTrackList();
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

            _position = config.Position;
        }
        #endregion

        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly Selector _selector;
        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _windowManager = windowManager;

            _selector = new Selector(_eventAggregator);

            Config config = FileManager.Import<Config>(ConfigPath);
            Initialize(config);
            _eventAggregator.PublishOnUIThreadAsync(config.SelectorOption);
            HistoryViewModel = new HistoryViewModel(_eventAggregator);
            FilterOptionIndicatorViewModel = new FilterOptionIndicatorViewModel(_eventAggregator);
            FilterOptionViewModel = new FilterOptionViewModel(_eventAggregator);

            _selector.Exclusions = config.Exclusions;
        }

        public Task HandleAsync(SelectorOption message, CancellationToken cancellationToken)
        {
            ChangeFilterView(message.FilterType);
            return Task.CompletedTask;
        }

        private void ChangeFilterView(string filterType)
        {
            FilterViewModel?.ExportFilter();
            FilterViewModel = filterType switch
            {
                nameof(ConditionalFilter) => new ConditionalFilterViewModel(_eventAggregator, _windowManager),
                nameof(SelectiveFilter) => new SelectiveFilterViewModel(_eventAggregator),
                _ => throw new NotSupportedException(),
            };
        }

        protected override void OnViewLoaded(object view)
        {
            _selector.AddHotKey();
        }

        private double[] _position;
        public void SetPosition(object view)
        {
            var window = view as Window;
            if (_position?.Length == 2)
            {
                window.Top = _position[0];
                window.Left = _position[1];
            }
        } 

        #region On Exit
        public void SaveConfig(object view)
        {
            var window = view as Window;

            FilterViewModel.ExportFilter();
            Config config = FileManager.Import<Config>(ConfigPath);

            config.FilterOption = FilterOptionViewModel.FilterOption;
            config.Exclusions = _selector.Exclusions;
            config.Position = new double[2] { window.Top, window.Left };
            FileManager.Export(config, ConfigPath);
        }
        #endregion

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
        public void ShowInfo()
        {
            _windowManager.ShowDialogAsync(_infoViewModel);
        }
        public void ShowSetting()
        {
            _windowManager.ShowDialogAsync(new SettingViewModel(_eventAggregator));
        }
        #endregion
    }
}
