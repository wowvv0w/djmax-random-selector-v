using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class HistoryViewModel : Screen, IHandle<PatternMessage>, IHandle<FilterOptionMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly TrackDB _db;

        private int _number;
        private bool _isFreeSelect;

        public BindableCollection<HistoryItem> History { get; }

        public HistoryViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _db = IoC.Get<TrackDB>();
            _number = 0;
            _isFreeSelect = false;
            History = new BindableCollection<HistoryItem>();
            DisplayName = "HISTORY";
        }

        private void AddItem(Pattern pattern)
        {
            _number++;
            string title = _db.Playable.First(track => track.Id == pattern.TrackId).Title;
            string style = _isFreeSelect ? "FREE" : pattern.Style;
            string level = _isFreeSelect ? "-" : pattern.Level.ToString();
            var historyItem = new HistoryItem()
            {
                Number = _number,
                Title = title,
                Style = style,
                Level = level,
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

        public Task HandleAsync(PatternMessage message, CancellationToken cancellationToken)
        {
            AddItem(message.Item);
            return Task.CompletedTask;
        }
        
        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            // TODO: is it work well on initializing?
            _isFreeSelect = message.MusicForm == MusicForm.Free;
            return Task.CompletedTask;
        }
    }
}
