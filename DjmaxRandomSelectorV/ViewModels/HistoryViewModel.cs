using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class HistoryViewModel : Screen, IHandle<MusicMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private int _number;

        public BindableCollection<HistoryItem> History { get; }

        public HistoryViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _number = 0;
            History = new BindableCollection<HistoryItem>();
            DisplayName = "HISTORY";
        }

        private void AddItem(Music music)
        {
            _number++;
            string style = music.Style;
            int level = music.Level;
            var historyItem = new HistoryItem()
            {
                Number = _number,
                Title = music.Title,
                Style = string.IsNullOrEmpty(style) ? "FREE" : style,
                Level = level == -1 ? "-" : level.ToString(),
                Time = DateTime.Now.ToString("HH:mm:ss"),
            };

            History.Insert(0, historyItem);
            if (History.Count > 10)
            {
                History.RemoveAt(10);
            }
        }

        public void ClearItems()
        {
            History.Clear();
            _number = 0;
        }

        public Task HandleAsync(MusicMessage message, CancellationToken cancellationToken)
        {
            AddItem(message.Item);
            return Task.CompletedTask;
        }
    }
}
