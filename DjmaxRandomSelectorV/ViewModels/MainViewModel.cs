using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DjmaxRandomSelectorV.Models;
using System.Threading;

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
            if (Selector.CanStart)
            {
                Thread thread = new Thread(new ThreadStart(() => Selector.Start(FilterViewModel.Filter)));
                Console.WriteLine("Start");
                thread.Start();
            }
            else
            {
                Console.WriteLine("Nope");
            }
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
