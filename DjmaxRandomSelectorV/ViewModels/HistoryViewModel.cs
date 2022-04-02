using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class HistoryViewModel : Screen
    {
        public BindableCollection<HistoryItem> History { get; set; }
        public void UpdateHistory(HistoryItem historyItem)
        {
            History.Insert(0, historyItem);
            if (History.Count > 8)
            {
                History.RemoveAt(8);
            }
        }

        public HistoryViewModel()
        {
            History = new BindableCollection<HistoryItem>();
        }
    }
}
