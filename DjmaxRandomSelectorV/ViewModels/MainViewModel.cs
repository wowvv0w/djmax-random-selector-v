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
using DjmaxRandomSelectorV.DataTypes.Interfaces;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>, IAddonObserver
    {
        private const int ApplicationVersion = 150;
        private const string DjmaxTitle = "DJMAX RESPECT V";
        
        private const string ReleasesUrl = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";

        private const string ConfigPath = "Data/Config.json";
        private const string CurrentFilterPath = "Data/CurrentFilter.json";
        private const string CurrentPlaylistPath = "Data/CurrentPlaylist.json";
        private const string AllTrackListPath = "Data/AllTrackList.csv";

        public FilterViewModel FilterViewModel { get; set; }
        public PlaylistViewModel PlaylistViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }
        public FilterOptionViewModel FilterOptionViewModel { get; set; }
        public FilterOptionIndicatorViewModel FilterOptionIndicatorViewModel { get; set; }

        private InfoViewModel _infoViewModel;

        private Filter _filter;
        private Playlist _playlist;
        private Config _config;

        private List<Track> _allTrackList;
        private List<Track> _trackList;
        private List<Music> _musicList;

        private bool _isUpdated;
        private bool _isRunning;

        private ISifter _sifter;
        private IProvider _provider;

        #region Initializing
        /// <summary>
        /// Compare to lastest versions of application and track list file from GitHub repository.
        /// </summary>
        /// <returns>Two 32-bit signed integers.</returns>
        private (int, int) CompareToLastestVersions()
        {
            using var client = new HttpClient();

            string result = client.GetStringAsync(VersionsUrl).Result;
            string[] versions = result.Split(',');

            var lastestAppVersion = int.Parse(versions[0]);
            var lastestTrackVersion = int.Parse(versions[1]);

            int gapWithLastestApp = lastestAppVersion - ApplicationVersion;
            int gapWithLastestTrack = lastestTrackVersion - _config.AllTrackVersion;

            return (gapWithLastestApp, gapWithLastestTrack);
        }
        /// <summary>
        /// Get track list file from GitHub repository.
        /// </summary>
        private void DownloadAllTrackList()
        {
            using var client = new HttpClient();
            string result = client.GetStringAsync(AllTrackListUrl).Result;

            using var writer = new StreamWriter(AllTrackListPath);
            writer.Write(result);
        }
        /// <summary>
        /// Get a list of all tracks from local track list file.
        /// </summary>
        private void GetAllTrackList()
        {
            using var reader = new StreamReader(AllTrackListPath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TrackMap>();
            var records = csv.GetRecords<Track>().ToList();
            
            _allTrackList = records;
        }
        /// <summary>
        /// Set <see cref="_trackList"/> according to <paramref name="ownedDlcs"/>.
        /// </summary>
        /// <param name="ownedDlcs">A list of <see cref="string"/> that contains categories.</param>
        public void UpdateTrackList(List<string> ownedDlcs)
        {
            var basicCategories = new List<string>() { "RP", "P1", "P2", "GG" };
            var titleFilter = CreateTitleFilter(ownedDlcs);
            var trackList = from track in _allTrackList
                            where (ownedDlcs.Contains(track.Category) || basicCategories.Contains(track.Category))
                            && !titleFilter.Contains(track.Title)
                            select track;

            _trackList = trackList.ToList();
            _isUpdated = true;
        }
        private List<string> CreateTitleFilter(List<string> ownedDlcs)
        {
            var list = new List<string>();

            if (!ownedDlcs.Contains("P3"))
            {
                list.Add("glory day (Mintorment Remix)");
                list.Add("glory day -JHS Remix-");
            }
            if (!ownedDlcs.Contains("TR"))
                list.Add("Nevermind");
            if (!ownedDlcs.Contains("CE"))
                list.Add("Rising The Sonic");
            if (!ownedDlcs.Contains("BS"))
                list.Add("ANALYS");
            if (!ownedDlcs.Contains("T1"))
                list.Add("Do you want it");
            if (!ownedDlcs.Contains("T2"))
                list.Add("End of Mythology");
            if (!ownedDlcs.Contains("T3"))
                list.Add("ALiCE");
            if (!ownedDlcs.Contains("TQ"))
                list.Add("Techno Racer");
            if (ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Here in the Moment ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Airwave ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && ownedDlcs.Contains("T1"))
                list.Add("SON OF SUN ~Extended Mix~");
            if (!ownedDlcs.Contains("VE") && ownedDlcs.Contains("VE2"))
                list.Add("너로피어오라 ~Original Ver.~");

            return list;
        }
        /// <summary>
        /// Initialize DJMAX Random Selector V.
        /// </summary>
        private void Initialize()
        {
            // Import Files.
            _filter = FileManager.Import<Filter>(CurrentFilterPath);
            _playlist = FileManager.Import<Playlist>(CurrentPlaylistPath);
            _config = FileManager.Import<Config>(ConfigPath);

            // Check if the files should be updated, and then create Info dialog.
            try
            {
                (int gapWithLastestApp, int gapWithLastestTrack) = CompareToLastestVersions();
                OpenReleasePageVisibility = gapWithLastestApp > 0 ? Visibility.Visible : Visibility.Hidden;
                if (gapWithLastestTrack != 0 || !File.Exists(AllTrackListPath))
                {
                    DownloadAllTrackList();
                    _config.AllTrackVersion += gapWithLastestTrack;
                    FileManager.Export(_config, ConfigPath);
                    MessageBox.Show($"All track list is updated to the version {_config.AllTrackVersion}.",
                        "DJMAX Random Selector V",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                _infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion + gapWithLastestApp,
                    _config.AllTrackVersion);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Cannot check available updates. Check the internet connection.",
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                _infoViewModel = new InfoViewModel(ApplicationVersion, ApplicationVersion,
                    _config.AllTrackVersion);
            }

            // Set track list and filtered music list.
            GetAllTrackList();
            UpdateTrackList(_config.OwnedDlcs);
        }
        #endregion

        public MainViewModel()
        {
            Initialize();

            FilterViewModel = new FilterViewModel(_filter, ShowFavorite);
            PlaylistViewModel = new PlaylistViewModel(_playlist, _trackList);
            HistoryViewModel = new HistoryViewModel();
            FilterOptionViewModel = new FilterOptionViewModel(_config);

            FilterOptionIndicatorViewModel = new FilterOptionIndicatorViewModel();

            _config.Subscribe(this);
            _config.Subscribe(FilterOptionIndicatorViewModel);
            _config.Notify();

            _isFilterType = !_config.IsPlaylist;
            _isPlaylistType = _config.IsPlaylist;

            SetPosition(_config.Position);
        }



        #region On Start Up
        public void AddHotkey(object view)
        {
            var window = view as Window;
            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
            RegisterHotKey(handle, HOTKEY_ID, 0x0000, KEY_F7);
        }
        #endregion

        #region Position
        private double _top;
        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }
        private double _left;
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }
        public void SetPosition(double[] position)
        {
            if (position.Length == 2)
            {
                Top = position[0];
                Left = position[1];
            }
        }
        #endregion

        #region Start Selector
        private void UpdateRecents<T>(List<T> recents, int filteredCount)
        {
            int recentsCount = recents.Count;
            int maxCount = _config.Except;
            if (recentsCount > maxCount)
            {
                recents.RemoveRange(0, recentsCount - maxCount);
            }
            else if (recentsCount >= filteredCount)
            {
                try
                {
                    recents.RemoveRange(0, recentsCount - filteredCount + 1);
                }
                catch (ArgumentException)
                {
                }
            }
        }
        private void Sift()
        {
            List<string> styles = new List<string>();
            foreach (string button in _filter.ButtonTunes)
                foreach (string difficulty in _filter.Difficulties)
                    styles.Add($"{button}{difficulty}");

            var trackList = from track in _trackList
                            where _filter.Categories.Contains(track.Category)
                                || _filter.IncludesFavorite && _config.Favorite.Contains(track.Title)
            select track;

            _musicList = _sifter.Sift(trackList.ToList(), styles, _filter.Levels, _filter.ScLevels);
        }
        private Music Pick(List<Music> musicList)
        {
            var random = new Random();
            var index = random.Next(musicList.Count - 1);
            var selectedMusic = musicList[index];

            return selectedMusic;
        }
        private void Provide(Music selectedMusic)
        {
            _provider.Provide(selectedMusic, _trackList, _config.InputDelay);
        }
        private bool CanStart()
        {
            string windowTitle = GetActiveWindowTitle();
            if (_config.Aider.Equals(Aider.Observe))
                return true;
            else if (!windowTitle.Equals(DjmaxTitle))
                throw new Exception("Foreground window is not DJMAX RESPECT V.");
            else if (!_isRunning)
                return true;
            else
                return false;
        }
        private void Start()
        {
            try
            {
                Music selectedMusic;
                if (_isFilterType)
                    selectedMusic = StartF();
                else
                    selectedMusic = StartP();

                var historyItem = new HistoryItem(selectedMusic);
                HistoryViewModel.UpdateHistory(historyItem);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("There is no music in filtered list.",
                    "Filter Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private Music StartF()
        {
            _isRunning = true;

            List<string> recents = _filter.Recents;

            if (_isUpdated || _filter.IsUpdated)
            {
                Sift();
                recents.Clear();
                _isUpdated = false;
            }
            else
            {
                var titleList = from music in _musicList
                                select music.Title;
                int titleCount = titleList.Distinct().Count();
                UpdateRecents(_filter.Recents, titleCount);
            }
            var musicList = from music in _musicList
                            where !recents.Contains(music.Title)
                            select music;

            Music selectedMusic;
            try
            {
                selectedMusic = Pick(musicList.ToList());
            }
            catch (ArgumentOutOfRangeException e)
            {
                _isRunning = false;
                throw e;
            }

            Provide(selectedMusic);
            recents.Add(selectedMusic.Title);
            _isRunning = false;
            return selectedMusic;
        }
        private Music StartP()
        {
            _isRunning = true;

            List<Music> recents = _playlist.Recents;

            if (_isUpdated || _playlist.IsUpdated)
            {
                _musicList = _playlist.MusicList;
                recents.Clear();
                _isUpdated = false;
            }
            else
            {
                UpdateRecents(_playlist.Recents, _playlist.MusicList.Count);
            }
            var musicList = from music in _musicList
                            where !recents.Contains(music)
                            select music;

            Music selectedMusic;
            try
            {
                selectedMusic = Pick(musicList.ToList());
            }
            catch (ArgumentOutOfRangeException e)
            {
                _isRunning = false;
                throw e;
            }

            Provide(selectedMusic);
            recents.Add(selectedMusic);
            _isRunning = false;
            return selectedMusic;
        }
        #endregion

        #region IAddonObserver Methods
        public void Update(IAddonObservable observable)
        {
            var setting = observable as Config;

            SetSifter(setting.Mode, setting.Level);
            SetProvider(setting.Mode, setting.Aider);
            _isUpdated = true;
        }
        private void SetSifter(Mode mode, Level level)
        {
            if (mode.Equals(Mode.Freestyle))
            {
                switch (level)
                {
                    case Level.Off:
                        _sifter = new Freestyle();
                        break;
                    case Level.Beginner:
                        _sifter = new FreestyleWithLevel("BEGINNER");
                        break;
                    case Level.Master:
                        _sifter = new FreestyleWithLevel("MASTER");
                        break;
                }
            }
            else
            {
                _sifter = new Online();
            }
        }
        private void SetProvider(Mode mode, Aider aider)
        {
            switch (aider)
            {
                case Aider.Off:
                    _provider = new Locator(false);
                    break;
                case Aider.AutoStart:
                    if (mode.Equals(Mode.Freestyle))
                        _provider = new Locator(true);
                    else
                        _provider = new Locator(false);
                    break;
                case Aider.Observe:
                    _provider = new Observe();
                    break;
            }
        }
        #endregion
        #region On Exit
        public void SaveConfig(object view)
        {
            var window = view as Window;

            FileManager.Export(_filter, CurrentFilterPath);
            FileManager.Export(_playlist, CurrentPlaylistPath);

            if (!_config.SavesRecents)
            {
                _filter.Recents.Clear();
                _playlist.Recents.Clear();
            }
            _config.Position = new double[2] { Top, Left };
            FileManager.Export(_config, ConfigPath);
        }
        #endregion

        private bool _isFilterType;
        private bool _isPlaylistType;
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
            _windowManager.ShowDialogAsync(new SettingViewModel(_config, UpdateTrackList, ChangeTypeOfFilter));
        }
        public void ShowFavorite()
        {
            Action<bool> setUpdated = value => _filter.IsUpdated = value;
            var titleList = _allTrackList.ConvertAll(x => x.Title).Distinct().ToList();
            _windowManager.ShowDialogAsync(new FavoriteViewModel(_config, titleList, setUpdated));
        }
        #endregion

        #region Other Constants & Methods
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;
        private const uint KEY_F7 = 118;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                int vkey = ((int)lParam >> 16) & 0xFFFF;
                if (vkey == KEY_F7)
                {
#if debug
                    if (!_selector.IsRunning)
                    {
                        Task task = new Task(() => Start());
                        task.Start();
                    }
                    else
                    {
                        Console.WriteLine("denied");
                    }
#else
                    try
                    {
                        if (CanStart())
                        {
                            Task task = new Task(() => Start());
                            task.Start();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message,
                            "Selector Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
#endif
                }

                handled = true;
            }
            return IntPtr.Zero;
        }
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
#endregion
    }
}
