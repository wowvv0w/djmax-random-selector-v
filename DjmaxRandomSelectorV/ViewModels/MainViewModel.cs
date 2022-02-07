using Caliburn.Micro;
using System;
using System.Collections;
using System.Windows;
using DjmaxRandomSelectorV.Models;
using static DjmaxRandomSelectorV.Models.Selector;

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

        public static void Start()
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
                MessageBox.Show("There is no music in filtered list.",
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
    }
}
