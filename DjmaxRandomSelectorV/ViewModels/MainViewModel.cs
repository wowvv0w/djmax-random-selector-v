using Caliburn.Micro;
using System;
using System.Collections;
using System.Windows;
using DjmaxRandomSelectorV.Models;
using static DjmaxRandomSelectorV.Models.Selector;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Forms;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public FilterViewModel FilterViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }
        public MainViewModel()
        {
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
                var selectedMusic = Pick();
                var historyItem = new HistoryItem(selectedMusic);
                HistoryViewModel.UpdateHistory(historyItem);

                var inputCommand = Find(selectedMusic);
                //Select(inputCommand);
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
                                if (CanStart)
                                {
                                    Thread thread = new Thread(new ThreadStart(() => StartSelector()));
                                    Console.WriteLine("Start");
                                    thread.Start();
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
    }
}
