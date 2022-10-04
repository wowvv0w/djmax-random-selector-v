using Caliburn.Micro;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Threading;
using Dmrsv.Data.Context.Schema;
using Dmrsv.RandomSelector;
using Dmrsv.Data.Controller;
using Dmrsv.RandomSelector.Assistants;
using System.Windows.Interop;
using Dmrsv.Data.Interfaces;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>, IHandle<IFilter>, IHandle<FilterOption>, IHandle<SelectorOption>
    {
        private const int ApplicationVersion = 150;
        private const string ReleasesUrl = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";

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

        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly Selector _selector;
        private readonly Executor _executor;
        public MainViewModel()
        {
            _eventAggregator = new EventAggregator();
            _eventAggregator.SubscribeOnUIThread(this);
            _windowManager = new WindowManager();

            _selector = new Selector();
            _executor = new Executor(_selector.CanStart, _selector.Start);
            _executor.ExecutionFailed += e => MessageBox.Show(e, "Selector Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _executor.ExecutionComplete += e => _eventAggregator.PublishOnUIThreadAsync(e);

            CheckUpdates();
            ChangeFilterView(new OptionApi().GetSelectorOption().FilterType);
            HistoryViewModel = new HistoryViewModel(_eventAggregator);
            FilterOptionIndicatorViewModel = new FilterOptionIndicatorViewModel(_eventAggregator);
            FilterOptionViewModel = new FilterOptionViewModel(_eventAggregator);
        }

        private void CheckUpdates()
        {
            try
            {
                using var client = new HttpClient();
                string result = client.GetStringAsync(VersionsUrl).Result;
                string[] versions = result.Split(',');
                var lastestAppVersion = int.Parse(versions[0]);
                var lastestTrackVersion = int.Parse(versions[1]);

                bool hasAppUpdate = lastestAppVersion > ApplicationVersion;
                OpenReleasePageVisibility = hasAppUpdate ? Visibility.Visible : Visibility.Hidden;

                var appOption = new OptionApi().GetAppOption();
                var currentTrackVersion = appOption.AllTrackVersion;
                bool hasTrackUpdate = lastestTrackVersion != currentTrackVersion;
                if (hasTrackUpdate || !File.Exists("Data/AllTrackList.csv"))
                {
                    new TrackApi().DownloadAllTrackList();
                    appOption.AllTrackVersion = lastestTrackVersion;
                    new OptionApi().SetAppOption(appOption);
                    MessageBox.Show($"All track list is updated to the version {lastestTrackVersion}.",
                        "DJMAX Random Selector V", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                _infoViewModel = new InfoViewModel(ApplicationVersion, lastestAppVersion);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Cannot check available updates. Check the internet connection.",
                    "Selector Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                _infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion);
            }
        }


        public Task HandleAsync(IFilter message, CancellationToken cancellationToken)
        {
            _selector.Handle(message);
            return Task.CompletedTask;
        }
        public Task HandleAsync(FilterOption message, CancellationToken cancellationToken)
        {
            _selector.Handle(message);
            return Task.CompletedTask;
        }
        public Task HandleAsync(SelectorOption message, CancellationToken cancellationToken)
        {
            _selector.Handle(message);
            ChangeFilterView(message.FilterType);
            return Task.CompletedTask;
        }

        private void ChangeFilterView(string filterType)
        {
            FilterViewModel = filterType switch
            {
                nameof(ConditionalFilter) => new ConditionalFilterViewModel(_eventAggregator, _windowManager),
                nameof(SelectiveFilter) => new SelectiveFilterViewModel(_eventAggregator),
                _ => throw new NotSupportedException(),
            };
        }

        protected override void OnViewLoaded(object view)
        {
            var window = view as Window;

            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(_executor.HwndHook);
            _executor.AddHotkey(handle, 9000, 0x0000, 118);

            SetPosition(window);
        }

        public void SetPosition(Window window)
        {
            var position = new OptionApi().GetAppOption().Position;
            if (position?.Length == 2)
            {
                window.Top = position[0];
                window.Left = position[1];
            }
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
            SaveConfig(window);
            window.Close();
        }
        private void SaveConfig(Window window)
        {
            var api = new OptionApi();
            var appOpt = api.GetAppOption();
            appOpt.Position = new double[2] { window.Top, window.Left };
            api.SetAppOption(appOpt);
            api.SaveConfig();
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
