using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class HistoryViewModel : Screen, IHandle<Music>
    {
        private int _number;
        public BindableCollection<HistoryItem> History { get; set; }

        private IEventAggregator _eventAggregator;
        public HistoryViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _number = 0;
            History = new BindableCollection<HistoryItem>();
        }

        private void UpdateHistory(HistoryItem historyItem)
        {
            History.Insert(0, historyItem);
            if (History.Count > 8)
            {
                History.RemoveAt(8);
            }
        }

        public Task HandleAsync(Music message, CancellationToken cancellationToken)
        {
            var historyItem = new HistoryItem()
            {
                Number = _number,
                Title = message.Title,
                Style = message.Style,
                Level = message.Level,
                Time = DateTime.Now.ToString("HH:mm:ss"),
            };
            UpdateHistory(historyItem);
            return Task.CompletedTask;
        }
    }
}
