using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using static DjmaxRandomSelectorV.Models.Selector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Effects;
using System.IO;
using DjmaxRandomSelectorV.Views;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private const int SELECTOR_VERSION = 102;

        private const string RELEASE_URL = "https://github.com/wowvv0w/djmax-random-selector-v/releases";

        private int _lastSelectorVer;
        private int _lastAllTrackVer;

        private DockPanel _dockPanel;
        private BlurEffect _blur = new BlurEffect() { Radius = 75 };

        public FilterViewModel FilterViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }

        private Advanced Advanced { get; set; }
        private Setting Setting { get; set; }

        public MainViewModel()
        {
            try
            {
                Setting = Manager.LoadSetting();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot Find config.json\nCreate new file.",
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                Setting = new Setting();
            }

            try
            {
                CheckUpdate();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                _lastSelectorVer = SELECTOR_VERSION;
            }

            try
            {
                Manager.ReadAllTrackList();
                Manager.UpdateTrackList(Setting.OwnedDlcs);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot Find AllTrackList.csv\nCreate new file.",
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                Manager.UpdateAllTrackList();
            }

            FilterViewModel = new FilterViewModel();
            HistoryViewModel = new HistoryViewModel();

            Advanced = new Advanced();
        }

        public void SetPos(object view)
        {
            var window = view as Window;

            Setting = Manager.LoadSetting();
            
            if (Setting.Position == null || Setting.Position.Length < 2)
            {
                Setting.Position = new double[2] { window.Top, window.Left };
            } else
            {
                window.Top = Setting.Position[0]; window.Left = Setting.Position[1];
            }
            Manager.SaveSetting(Setting);
        }

        private void CheckUpdate()
        {
            (_lastSelectorVer, _lastAllTrackVer) = Manager.GetLastVersions();

            if (SELECTOR_VERSION < _lastSelectorVer)
            {
                OpenReleasePageVisibility = Visibility.Visible;
            }

            if (Setting.AllTrackVersion < _lastAllTrackVer)
            {
                Manager.UpdateAllTrackList();
                Setting.AllTrackVersion = _lastAllTrackVer;
                Manager.SaveSetting(Setting);
            }
        }

        private void Start()
        {
            CanStart = false;
            if (IsFilterChanged)
            {
                SiftOut(FilterViewModel.Filter);
                IsFilterChanged = false;
            }

            List<string> recents = Advanced.Recents;
            recents = CheckRecents(recents);

            try
            {
                Music selectedMusic = Pick(recents);

                InputCommand inputCommand = Find(selectedMusic);
                Select(inputCommand, Setting.InputDelay);

                var historyItem = new HistoryItem(selectedMusic);
                HistoryViewModel.UpdateHistory(historyItem);

                recents.Add(selectedMusic.Title);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("There is no music in filtered list.",
                    "Filter Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            CanStart = true;
        }

        public void CloseEvent(object view)
        {
            var window = view as Window;

            Manager.SavePreset(FilterViewModel.Filter);

            Setting = Manager.LoadSetting();
            Setting.Position = new double[2] { (window.Top < 0 ? 0 : window.Top), (window.Left < 0 ? 0 : window.Left) };
            Manager.SaveSetting(Setting);
        }


        public void OpenReleasePage()
        {
            System.Diagnostics.Process.Start(RELEASE_URL);
        }

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


        // Window Bar
        public void MoveWindow(object view)
        {
            var window = view as Window;
            window.DragMove();
        }

        // Window Buttons
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

        public void GetDockPanel(object source)
        {
            _dockPanel = source as DockPanel;
        }

        private string _currentTab = "FILTER";
        public string CurrentTab
        {
            get { return _currentTab; }
            set 
            { 
                _currentTab = value; 
                NotifyOfPropertyChange(() => CurrentTab);
            }
        }

        // Tab Buttons
        private bool _isFilterTabSelected = true;
        private bool _isHistoryTabSelected = false;
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

        // Utility Buttons
        IWindowManager windowManager = new WindowManager();
        public void ShowSetting()
        {
            _dockPanel.Effect = _blur;
            windowManager.ShowDialogAsync(new SettingViewModel(Setting, _dockPanel));
        }
        public void ShowInfo()
        {
            _dockPanel.Effect = _blur;
            var infoViewModel
                = new InfoViewModel(SELECTOR_VERSION, _lastSelectorVer, Setting.AllTrackVersion, _dockPanel);
            windowManager.ShowDialogAsync(infoViewModel);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;
        private const uint KEY_F7 = 118;
        private const uint KEY_F2 = 113;
        private const string DJMAX_TITLE = "DJMAX RESPECT V";

        public void AddHotKey(object view)
        {
            var window = view as Window;

            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            RegisterHotKey(handle, HOTKEY_ID, 0x0000, KEY_F7);
            RegisterHotKey(handle, HOTKEY_ID, 0x0000, KEY_F2);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                string windowTitle = GetActiveWindowTitle();
                
                int vkey = ((int)lParam >> 16) & 0xFFFF;
                if (vkey == KEY_F7)
                {
                    if (CanStart && windowTitle == DJMAX_TITLE)
                    {
                        Thread thread = new Thread(new ThreadStart(() => Start()));
                        thread.Start();
                    }
                    else if (windowTitle != DJMAX_TITLE)
                    {
                        MessageBox.Show("Foreground window is not DJMAX RESPECT V.",
                            "Selector Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else if (vkey == KEY_F2)
                {
                    if (windowTitle == DJMAX_TITLE)
                    {
                        var view = GetView() as MainView;
                        var window = view as Window;

                        if (window.WindowState != WindowState.Minimized)
                        {
                            window.WindowState = WindowState.Minimized;
                        }
                        else
                        {
                            window.WindowState = WindowState.Normal;
                            window.Activate();
                        }
                    }
                }

                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

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
    }
}
