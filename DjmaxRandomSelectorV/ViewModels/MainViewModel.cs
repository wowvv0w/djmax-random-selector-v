//#define debug

using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.DataTypes.Enums;
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

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private const int SELECTOR_VERSION = 140;
        private const string RELEASE_URL = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
        private const string VersionUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        private const string DjmaxTitle = "DJMAX RESPECT V";

        private int lastSelectorVer;

        private DockPanel dockPanel;

        public FilterViewModel FilterViewModel { get; set; }
        public PlaylistViewModel PlaylistViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }

        public AddonViewModel AddonPanel { get; set; }
        public AddonViewModel AddonButton { get; set; }

        private readonly Filter filter;
        private readonly Playlist _playlist;
        private readonly Selector selector;
        private readonly Setting setting;

        public MainViewModel()
        {
            setting = new Setting();
            setting.Import();

            selector = new Selector();
            try
            {
                int _lastAllTrackVer;
                using (var client = new HttpClient())
                {
                    var data = client.GetStringAsync(VersionUrl).Result;
                    var versions = data.Split(',');

                    lastSelectorVer = int.Parse(versions[0]);
                    _lastAllTrackVer = int.Parse(versions[1]);
                }

                if (SELECTOR_VERSION < lastSelectorVer)
                    OpenReleasePageVisibility = Visibility.Visible;

                if (setting.AllTrackVersion != _lastAllTrackVer)
                {
                    selector.DownloadAllTrackList();
                    setting.AllTrackVersion = _lastAllTrackVer;
                    setting.Export();
                    MessageBox.Show($"All track list is updated to the version {_lastAllTrackVer}.",
                        "DJMAX Random Selector V",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                lastSelectorVer = SELECTOR_VERSION;
            }

            try
            {
                selector.ReadAllTrackList();
                selector.UpdateTrackList(setting.OwnedDlcs);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot Find AllTrackList.csv\nCreate new file.",
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                selector.DownloadAllTrackList();
            }

            _playlist = new Playlist();
            _playlist.Import();

            FilterViewModel = new FilterViewModel(ShowFavorite);
            PlaylistViewModel = new PlaylistViewModel(_playlist, selector.TrackList);
            HistoryViewModel = new HistoryViewModel();
            filter = FilterViewModel.Filter;

            AddonPanel = new AddonViewModel();
            AddonButton = new AddonViewModel();
            AddonPanel.ExceptCount = setting.RecentsCount;
            AddonButton.ExceptCount = setting.RecentsCount;
            SetAddonText(setting.Mode);
            SetAddonText(setting.Aider);
            SetAddonText(setting.Level);

            setting.Subscribe(selector);
            setting.Subscribe(AddonPanel);
            setting.Subscribe(AddonButton);
            setting.Notify();

            _isFilterType = !setting.IsPlaylist;
            _isPlaylistType = setting.IsPlaylist;
        }


        #region On Start Up
        public void ShowEvent(object view)
        {
            var window = view as Window;
            AddHotKey(window);
            SetPosition(window);
        }
        private void AddHotKey(Window window)
        {
            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
            RegisterHotKey(handle, HOTKEY_ID, 0x0000, KEY_F7);
        }
        private void SetPosition(Window window)
        {
            if (setting.Position.Length == 2)
            {
                window.Top = setting.Position[0];
                window.Left = setting.Position[1];
            }
            else
            {
                setting.Position = new double[2] { window.Top, window.Left };
            }
        }
        public void GetDockPanel(object source)
        {
            var dockPanel = source as DockPanel;
            this.dockPanel = dockPanel;
        }
        #endregion


        #region Start Selector
        private bool CanStart()
        {
            string windowTitle = GetActiveWindowTitle();
            if (setting.Aider.Equals(Aider.Observe))
                return true;
            else if (!windowTitle.Equals(DjmaxTitle))
                throw new Exception("Foreground window is not DJMAX RESPECT V.");
            else if (!selector.IsRunning)
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
                    selectedMusic = selector.Start(filter, setting);
                else
                    selectedMusic = selector.Start(_playlist, setting);

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
        #endregion

        #region On Exit
        public void CloseEvent(object view)
        {
            var window = view as Window;

            if (!setting.SavesRecents)
            {
                FilterViewModel.Filter.Recents.Clear();
                _playlist.Recents.Clear();
            }
            FilterViewModel.Filter.Export();
            _playlist.Export();

            setting.Position = new double[2] { (window.Top < 0 ? 0 : window.Top), (window.Left < 0 ? 0 : window.Left) };
            setting.Export();
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
        private Visibility _openReleasePageVisibility = Visibility.Hidden;
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
            System.Diagnostics.Process.Start(RELEASE_URL);
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
            SetBlurEffect(true);
            var infoViewModel
                = new InfoViewModel(SELECTOR_VERSION, lastSelectorVer, setting.AllTrackVersion, SetBlurEffect);
            _windowManager.ShowDialogAsync(infoViewModel);
        }
        public void ShowSetting()
        {
            SetBlurEffect(true);
            _windowManager.ShowDialogAsync(new SettingViewModel(setting, SetBlurEffect, selector.UpdateTrackList, ChangeTypeOfFilter));
        }
        public void ShowFavorite()
        {
            SetBlurEffect(true);
            Action<bool> setUpdated = value => filter.IsUpdated = value;
            _windowManager.ShowDialogAsync(new FavoriteViewModel(setting, selector.GetTitleList(), SetBlurEffect, setUpdated));
        }
        #endregion


        #region Equipment

        #region Show/Hide
        public void ShowEquipment() => SetBlurEffect(true);
        public void HideEquipment() => SetBlurEffect(false);
        private void SetBlurEffect(bool turnsOn)
        {
            dockPanel.Effect = turnsOn ? new BlurEffect() { Radius = 75 } : null;
        }
        #endregion
        #region Constants
        private const string OFF = "OFF";
        private const string FREESTYLE = "FREESTYLE";
        private const string ONLINE = "ONLINE";
        private const string AUTO_START = "AUTO START";
        private const string OBSERVE = "OBSERVE";
        private const string BEGINNER = "BEGINNER";
        private const string MASTER = "MASTER";
        #endregion        

        public int RecentsCount
        {
            get { return setting.RecentsCount; }
            set
            {
                setting.RecentsCount = value;
                NotifyOfPropertyChange(() => RecentsCount);
                AddonPanel.ExceptCount = value;
                AddonButton.ExceptCount = value;
            }
        }

        private string modeText;
        public string ModeText
        {
            get { return modeText; }
            set
            {
                modeText = value;
                NotifyOfPropertyChange(() => ModeText);
            }
        }
        private void SetAddonText(Mode mode)
        {
            switch (mode)
            {
                case Mode.Freestyle:
                    ModeText = FREESTYLE;
                    break;
                case Mode.Online:
                    ModeText = ONLINE;
                    break;
            }
        }
        public void SwitchMode()
        {
            if (setting.Mode.Equals(Mode.Freestyle))
                setting.Mode = Mode.Online;
            else
                setting.Mode = Mode.Freestyle;
            SetAddonText(setting.Mode);
        }

        private string aiderText;
        public string AiderText
        {
            get { return aiderText; }
            set
            {
                aiderText = value;
                NotifyOfPropertyChange(() => AiderText);
            }
        }
        private void SetAddonText(Aider aider)
        {
            switch (aider)
            {
                case Aider.Off:
                    AiderText = OFF;
                    break;
                case Aider.AutoStart:
                    AiderText = AUTO_START;
                    break;
                case Aider.Observe:
                    AiderText = OBSERVE;
                    break;
            }
        }
        public void PrevAider()
        {
            if (setting.Aider.Equals(Aider.Off))
                setting.Aider = Aider.Observe;
            else
                setting.Aider--;
            SetAddonText(setting.Aider);
        }
        public void NextAider()
        {
            if (setting.Aider.Equals(Aider.Observe))
                setting.Aider = Aider.Off;
            else
                setting.Aider++;
            SetAddonText(setting.Aider);
        }

        private string levelText;
        public string LevelText
        {
            get { return levelText; }
            set
            {
                levelText = value;
                NotifyOfPropertyChange(() => LevelText);
            }
        }
        private void SetAddonText(Level level)
        {
            switch (level)
            {
                case Level.Off:
                    LevelText = OFF;
                    break;
                case Level.Beginner:
                    LevelText = BEGINNER;
                    break;
                case Level.Master:
                    LevelText = MASTER;
                    break;
            }
        }
        public void PrevLevel()
        {
            if (setting.Level.Equals(Level.Off))
                setting.Level = Level.Master;
            else
                setting.Level--;
            SetAddonText(setting.Level);
        }
        public void NextLevel()
        {
            if (setting.Level.Equals(Level.Master))
                setting.Level = Level.Off;
            else
                setting.Level++;
            SetAddonText(setting.Level);
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
