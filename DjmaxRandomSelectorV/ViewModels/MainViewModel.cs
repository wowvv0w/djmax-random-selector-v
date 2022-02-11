using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Properties;
using System;
using System.Collections;
using System.Windows;
using static DjmaxRandomSelectorV.Models.Selector;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private const int SELECTOR_VERSION = 1000;

        private const string RELEASE_URL = "https://github.com/wowvv0w/djmax-random-selector-v/releases";

        private int _lastSelectorVersion;
        private int _lastAllTrackVersion;

        public FilterViewModel FilterViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }

        public MainViewModel()
        {
            try
            {
                (_lastSelectorVersion, _lastAllTrackVersion) = Manager.UpdateCheck();
                if (SELECTOR_VERSION < _lastSelectorVersion)
                {
                    OpenReleasePageVisibility = Visibility.Visible;
                }
                if (Settings.Default.allTrackVersion < _lastAllTrackVersion)
                {
                    Manager.UpdateAllTrackList();
                    Settings.Default.allTrackVersion = _lastAllTrackVersion;
                    Settings.Default.Save();
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Cannot check last version.",
                    "Selector Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                _lastSelectorVersion = SELECTOR_VERSION;
            }

            Manager.ReadAllTrackList();
            Manager.UpdateTrackList();

            FilterViewModel.Filter = Manager.LoadPreset();

            FilterViewModel = new FilterViewModel();
            HistoryViewModel = new HistoryViewModel();
        }

        public void StartSelector()
        {
            CanStart = false;
            if (IsFilterChanged)
            {
                SiftOut(FilterViewModel.Filter);
                IsFilterChanged = false;
            }

            try
            {
                var recents = FilterViewModel.Filter.Recents;
                recents = CheckRecents(recents);

                var selectedMusic = Pick(recents);

                var inputCommand = Find(selectedMusic);
                Select(inputCommand);

                var historyItem = new HistoryItem(selectedMusic);
                HistoryViewModel.UpdateHistory(historyItem);

                recents.Add(selectedMusic.Title);
            }
            catch (ArgumentOutOfRangeException)
            {
                System.Windows.MessageBox.Show("There is no music in filtered list.",
                    "Filter Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            CanStart = true;
        }

        public void CloseEvent()
        {
            Manager.SavePreset(FilterViewModel.Filter);
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
        public void ShowOption()
        {
            windowManager.ShowDialogAsync(new OptionViewModel());
        }
        public void ShowInfo()
        {
            var infoViewModel = new InfoViewModel(SELECTOR_VERSION, _lastSelectorVersion);
            windowManager.ShowDialogAsync(infoViewModel);
        }




        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        private const int HOTKEY_ID = 9000;

        public void AddHotKey(object view)
        {
            Window window = view as Window;

            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            RegisterHotKey(handle, HOTKEY_ID, 0x0000, (uint)Keys.F7);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == (uint)Keys.F7)
                            {
                                var windowTitle = GetActiveWindowTitle();
                                Console.WriteLine(windowTitle);
                                if (CanStart && windowTitle == "DJMAX RESPECT V")
                                {
                                    Thread thread = new Thread(new ThreadStart(() => StartSelector()));
                                    Console.WriteLine("Start");
                                    thread.Start();
                                }
                                else if (windowTitle != "DJMAX RESPECT V")
                                {
                                    System.Windows.MessageBox.Show("Active window is not DJMAX RESPECT V.",
                                        "Selector Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                                }
                                else
                                {
                                    Console.WriteLine("Nope");
                                }
                            }
                            handled = true;
                            break;
                    }
                    break;
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
