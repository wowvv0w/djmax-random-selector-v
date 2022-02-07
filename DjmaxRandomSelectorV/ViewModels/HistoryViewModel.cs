using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class HistoryViewModel : Screen
    {
        public BindableCollection<HistoryItem> History { get; set; }
        public void UpdateHistory(HistoryItem historyItem)
        {
            History.Add(historyItem);
            if (History.Count > 8)
            {
                History.RemoveAt(0);
            }
        }

        public HistoryViewModel()
        {
            History = new BindableCollection<HistoryItem>();
        }
    }
}
