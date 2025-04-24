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

        public RandomSelector(IEventAggregator eventAggregator, TrackDB db)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _db = db;
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
            _locator.MakeLocations(_db.AllTrack);
            SetLocatorProperties(new FilterOptionMessage(
                filterOption.RecentsCount,
                filterOption.MusicForm,
                filterOption.InputMethod,
                filterOption.LevelPreference));
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
            if (selected is null)
            {
                ShowErrorMessageBox("There is no music that meets the filter conditions.");
                return;
            }
            _locator.Locate(selected);
            _eventAggregator.PublishOnUIThreadAsync(new PatternMessage(selected));
            _isRunning = false;
        }

        private void UpdateCandidates()
        {
            _candidates = _picker.Pick(_filter.Filter(_db.Playable)).ToList();
            _history.Clear();
        }

        private void SetLocatorProperties(FilterOptionMessage message)
        {
            _locator.CanLocate = message.InputMethod != InputMethod.NotInput;
            _locator.LocatesStyle = message.MusicForm == MusicForm.Default;
            _locator.PressesStart = message.MusicForm == MusicForm.Default && message.InputMethod == InputMethod.WithAutoStart;
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
            SetLocatorProperties(message);
            _picker.SetPickMethod(message.MusicForm, message.LevelPreference);
            UpdateCandidates();
            return Task.CompletedTask;
        }

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            _locator.InputInterval = message.InputInterval;
            _db.SetPlayable(message.OwnedDlcs);
            _locator.MakeLocations(_db.AllTrack);
            UpdateCandidates();
            return Task.CompletedTask;
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
