using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DjmaxRandomSelectorV
{
    public class RandomSelector : IHandle<FilterMessage>, IHandle<FilterOptionMessage>, IHandle<SettingMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly TrackDB _db;
        private List<Pattern> _candidates;

        private bool _isRunning;

        private IFilter _filter;
        private PatternPicker _picker;
        private IHistory<int> _history;
        private ISelector _selector;
        private Locator _locator;

        private readonly WindowTitleHelper _windowTitleHelper;

        public IHistory<int> History => _history;

        public RandomSelector(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _db = IoC.Get<TrackDB>();
            _isRunning = false;
            _windowTitleHelper = new WindowTitleHelper();
        }

        public void Initialize(Dmrsv3Configuration config)
        {
            var setting = config.Setting;
            var filterOption = config.FilterOption;
            _candidates = new List<Pattern>();
            _picker = new PatternPicker();
            _history = new History<int>(setting.RecentPlayed, filterOption.RecentsCount);
            _selector = new SelectorWithHistory(_history);
            _locator = new Locator();
        }

        public bool CanStart()
        {
            if (!_windowTitleHelper.EqualsDjmax())
            {
                ShowErrorMessageBox("The foreground window is not \"DJMAX RESPECT V\".\nPress start key in the game.");
                return false;
            }
            if (!_isRunning)
            {
                return true;
            }
            return false;
        }
        public void Start()
        {
            _isRunning = true;
            if (_filter.IsUpdated)
            {
                UpdateCandidates();
            }
            Pattern selected = _selector.Select(_candidates);
            if (selected is not null)
            {
                _locator.Locate(selected, _db.Playable);
                _eventAggregator.PublishOnUIThreadAsync(new PatternMessage(selected));
            }
            else
            {
                ShowErrorMessageBox("There is no music that meets the filter conditions.");
            }
            _isRunning = false;
        }

        private void UpdateCandidates()
        {
            _candidates = _picker.Pick(_filter.Filter(_db.Playable)).ToList();
            _history.Clear();
        }


        public Task HandleAsync(FilterMessage message, CancellationToken cancellationToken)
        {
            _filter = message.Item;
            return Task.CompletedTask;
        }
        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            if (_history.Capacity != message.RecentsCount)
            {
                _history.Capacity = message.RecentsCount;
            }
            _locator.SetStartsAutomatically(message.MusicForm, message.InputMethod);
            _locator.SetInvokesInput(message.InputMethod);
            _picker.SetPickMethod(message.MusicForm, message.LevelPreference);
            return Task.CompletedTask;
        }

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            _locator.SetInputInterval(message.InputInterval);
            _db.SetPlayable(message.OwnedDlcs);
            UpdateCandidates();
            return Task.CompletedTask;
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
