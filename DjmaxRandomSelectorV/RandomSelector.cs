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

        private IEnumerable<Track> _playable;
        private IEnumerable<Music> _candidates;

        private bool _isRunning;

        private IFilter _filter;
        private Func<IEnumerable<Music>, IEnumerable<Music>> _outputMethod;
        private IRecent<string> _recent;
        private ISelector _selector;
        private Locator _keyInputInvoker;

        private readonly WindowTitleHelper _windowTitleHelper;
        private readonly ExecutionHelper _executionHelper;

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

            _recent = new RecentHelper<string>(config.Exclusions, config.RecentsCount);
            _selector = new SelectorWithRecent(_recent);

            _keyInputInvoker = new Locator()
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
                _recent.Clear();
            }
            var selected = _selector.Select(_candidates);
            if (selected is not null)
            {
                _keyInputInvoker?.Provide(selected, _playable.ToList());
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

        public IEnumerable<Track> GetAllTrack()
        {
            return new TrackManager().GetAllTrack();
        }

        public Task HandleAsync(FilterMessage message, CancellationToken cancellationToken)
        {
            _filter = message.Item;
            _filter.OutputMethod = _outputMethod;
            return Task.CompletedTask;
        }

        public Task HandleAsync(CapacityMessage message, CancellationToken cancellationToken)
        {
            _recent.Capacity = message.Value;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ModeWithAiderMessage message, CancellationToken cancellationToken)
        {
            InputMethod aider = message.Aider;
            _keyInputInvoker.StartsAutomatically = message.Mode == MusicForm.Default && aider == InputMethod.WithAutoStart;
            _keyInputInvoker.InvokesInput = aider != InputMethod.NotInput;
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
            _keyInputInvoker.InputInterval = message.InputInterval;
            _playable = new TrackManager().CreateTracks(message.OwnedDlcs);
            return Task.CompletedTask;
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
