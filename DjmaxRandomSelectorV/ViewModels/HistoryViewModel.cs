using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using System;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class HistoryViewModel : Screen
    {
        private int _number;

        public BindableCollection<HistoryItem> History { get; set; }

        public HistoryViewModel()
        {
            _number = 0;
            History = new BindableCollection<HistoryItem>();
            DisplayName = "HISTORY";
        }

        public void AddItem(Music music)
        {
            _number++;
            var historyItem = new HistoryItem()
            {
                Number = _number,
                Title = music.Title,
                Style = $"{music.ButtonTunes}{music.Difficulty}",
                Level = music.Level.ToString(),
                Time = DateTime.Now.ToString("HH:mm:ss"),
            };

            History.Insert(0, historyItem);
            if (History.Count > 8)
            {
                History.RemoveAt(8);
            }
        }
    }
}
