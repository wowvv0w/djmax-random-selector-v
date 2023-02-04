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
    public class RandomSelector : IHandle<FilterMessage>, IHandle<CapacityMessage>,
        IHandle<ModeWithAiderMessage>, IHandle<ModeWithLevelMessage>, IHandle<SettingMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private List<Track> _playable;
        private List<Music> _candidates;

        private bool _isRunning;

        private IFilter _filter;
        private OutputMethodCallback _outputMethod;
        private IHistory<string> _history;
        private ISelector _selector;
        private ILocator _locator;

        private readonly WindowTitleHelper _windowTitleHelper;
        private readonly ExecutionHelper _executionHelper;

        public IHistory<string> History => _history;

        public RandomSelector(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _isRunning = false;
            _windowTitleHelper = new WindowTitleHelper();
            _executionHelper = new ExecutionHelper(CanStart, Start);
        }

        public void Initialize(Configuration config)
        {
            _playable = new TrackManager().CreateTracks(config.OwnedDlcs);
            _candidates = new List<Music>();

            _outputMethod = new OutputMethodCreator().Create(config.Mode, config.Level);

            _history = new History<string>(config.Exclusions, config.RecentsCount);
            _selector = new SelectorWithHistory(_history);

            _locator = new Locator()
            {
                StartsAutomatically = config.Mode == MusicForm.Default && config.Aider == InputMethod.WithAutoStart,
                InputInterval = config.InputDelay,
                InvokesInput = config.Aider != InputMethod.NotInput,
            };
            _executionHelper.IgnoreCanExecute = config.Aider == InputMethod.NotInput;
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
                _candidates = _filter.Filter(_playable);
                _history.Clear();
            }
            var selected = _selector.Select(_candidates);
            if (selected is not null)
            {
                _locator.Locate(selected, _playable);
                _eventAggregator.PublishOnUIThreadAsync(new MusicMessage(selected));
            }
            else
            {
                ShowErrorMessageBox("There is no music that meets the filter conditions.");
            }
            _isRunning = false;
        }

        public void RegisterHandle(IntPtr handle)
        {
            HwndSource source = HwndSource.FromHwnd(handle);
            _executionHelper.Register(handle, 9000);
            source.AddHook(_executionHelper.HwndHook);
        }
        public void SetHotkey(uint fsModifiers, uint vk)
        {
            _executionHelper.SetHotkey(fsModifiers, vk);
        }

        public Task HandleAsync(FilterMessage message, CancellationToken cancellationToken)
        {
            _filter = message.Item;
            _filter.OutputMethod = _outputMethod;
            return Task.CompletedTask;
        }

        public Task HandleAsync(CapacityMessage message, CancellationToken cancellationToken)
        {
            _history.Capacity = message.Value;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ModeWithAiderMessage message, CancellationToken cancellationToken)
        {
            InputMethod aider = message.Aider;
            _locator.StartsAutomatically = message.Mode == MusicForm.Default && aider == InputMethod.WithAutoStart;
            _locator.InvokesInput = aider != InputMethod.NotInput;
            _executionHelper.IgnoreCanExecute = aider == InputMethod.NotInput;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ModeWithLevelMessage message, CancellationToken cancellationToken)
        {
            _outputMethod = new OutputMethodCreator().Create(message.Mode, message.Level);
            _filter.OutputMethod = _outputMethod;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            _locator.InputInterval = message.InputInterval;
            _playable = new TrackManager().CreateTracks(message.OwnedDlcs);
            _candidates = _filter.Filter(_playable);
            _history.Clear();
            return Task.CompletedTask;
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
