using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public FilterViewModel FilterViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }
        public MainViewModel()
        {
            FilterViewModel = new FilterViewModel();
            HistoryViewModel = new HistoryViewModel();
        }

        /// Menu Bar
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
        }
        public void LoadHistoryTab()
        {
            IsHistoryTabSelected = true;
        }
    }
}
