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

        public AddonViewModel AddonViewModel { get; set; }
        public AddonViewModel AddonButton { get; set; }

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

            AddonViewModel = new AddonViewModel(Setting);
            AddonButton = new AddonViewModel(Setting);

            UpdateAddon(Setting.Aider);
        }

        public void SetPos(object view)
        {
            var window = view as Window;
            
            if (Setting.Position == null || Setting.Position.Length < 2)
            {
                Setting.Position = new double[2] { window.Top, window.Left };
            } else
            {
                window.Top = Setting.Position[0]; window.Left = Setting.Position[1];
            }
        }

        private void CheckUpdate()
        {
            (_lastSelectorVer, _lastAllTrackVer) = Manager.GetLastVersions();

            if (SELECTOR_VERSION < _lastSelectorVer)
            {
                OpenReleasePageVisibility = Visibility.Visible;
            }

            if (Setting.AllTrackVersion != _lastAllTrackVer)
            {
                Manager.UpdateAllTrackList();
                Setting.AllTrackVersion = _lastAllTrackVer;
                Manager.SaveSetting(Setting);
            }
        }

        private void Start()
        {
            CanStart = false;
            Filter filter = FilterViewModel.Filter;

            var favorite = filter.IncludesFavorite ? Setting.Favorite : new List<string>();
            List<string> recents = filter.Recents;

            if (IsFilterChanged)
            {
                SiftOut(filter, favorite);
                recents.Clear();
                IsFilterChanged = false;
            }
            else
            {
                recents = CheckRecents(recents, Setting.RecentsCount);
            }

            try
            {
                Music selectedMusic = Pick(recents);

                InputCommand inputCommand = Find(selectedMusic);
                inputCommand.Delay = Setting.InputDelay;
                inputCommand.Starts = (Setting.Aider == Aider.AutoStart);
                Select(inputCommand);

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

            if (!Setting.SavesRecents)
            {
                FilterViewModel.Filter.Recents.Clear();
            }
            Manager.SavePreset(FilterViewModel.Filter);

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



        IWindowManager windowManager = new WindowManager();
        public void ShowInfo()
        {
            SetBlurEffect();
            var infoViewModel
                = new InfoViewModel(SELECTOR_VERSION, _lastSelectorVer, Setting.AllTrackVersion, _dockPanel);
            windowManager.ShowDialogAsync(infoViewModel);
        }
        public void ShowSetting()
        {
            SetBlurEffect();
            windowManager.ShowDialogAsync(new SettingViewModel(Setting, _dockPanel));
        }
        public void ShowInventory()
        {
            SetBlurEffect();
            windowManager.ShowDialogAsync(new InventoryViewModel(Setting, _dockPanel));
        }


        #region Equipment
        public void ShowEquipment()
        {
            SetBlurEffect();
        }
        public void HideEquipment()
        {
            SetBlurEffect(false);
        }

        public int RecentsCount
        {
            get => Setting.RecentsCount;
            set
            {
                Setting.RecentsCount = value;
                NotifyOfPropertyChange(() => RecentsCount);
                AddonViewModel.ExceptCount = value;
                AddonButton.ExceptCount = value;
            }
        }

        private const string OFF = "OFF";
        private const string AUTO_START = "AUTO START";

        private string _aiderText;
        public string AiderText
        {
            get => _aiderText;
            set
            {
                _aiderText = value;
                NotifyOfPropertyChange(() => AiderText);
            }
        }
        
        private void UpdateAddon(Aider aider)
        {
            switch (aider)
            {
                case Aider.Off: AiderText = OFF; break;
                case Aider.AutoStart: AiderText = AUTO_START; break;
            }
            AddonViewModel.SetBitmapImage(aider);
            AddonButton.SetBitmapImage(aider);
        }
        public void PrevAider()
        {
            if (Setting.Aider == Aider.Off)
            {
                Setting.Aider = Aider.AutoStart;
            }
            else
            {
                Setting.Aider = Aider.Off;
            }
            UpdateAddon(Setting.Aider);
        }
        public void NextAider()
        {
            if (Setting.Aider == Aider.AutoStart)
            {
                Setting.Aider = Aider.Off;
            }
            else
            {
                Setting.Aider = Aider.AutoStart;
            }
            UpdateAddon(Setting.Aider);
        }

        public void SetBlurEffect(bool turnsOn = true)
        {
            _dockPanel.Effect = turnsOn ? _blur : null;
        }

        #endregion


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

        private bool _MainViewTopmost = false;
        public bool MainViewTopmost
        {
            get { return _MainViewTopmost; }
            set
            {
                _MainViewTopmost = value;
                NotifyOfPropertyChange(() => MainViewTopmost);
            }
        }

        private WindowState _MainViewState = WindowState.Normal;
        public WindowState MainViewState
        {
            get { return _MainViewState; }
            set
            {
                _MainViewState = value;
                NotifyOfPropertyChange(() => MainViewState);
            }
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
                        if (MainViewState != WindowState.Minimized)
                        {
                            MainViewState = WindowState.Minimized;
                            MainViewTopmost = false;
                        }
                        else
                        {
                            MainViewState = WindowState.Normal;
                            MainViewTopmost = true;
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
