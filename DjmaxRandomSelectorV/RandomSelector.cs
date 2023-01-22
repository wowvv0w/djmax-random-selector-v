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
    public class RandomSelector : IHandle<FilterOptionMessage>, IHandle<SettingMessage>
    {
        public delegate void MusicEventHandler(Music e);
        public event MusicEventHandler OnSelectionSuccessful;

        private IEnumerable<Track> _playable;
        private IEnumerable<Music> _candidates;

        private bool _isRunning;

        private IFilter _filter;
        private IRecent<string> _recent;
        private ISelector _selector;
        private Locator _keyInputInvoker;

        private readonly WindowTitleHelper _windowTitleHelper;
        private readonly ExecutionHelper _executionHelper;

        public IFilter Filter => _filter;

        public RandomSelector()
        {
            _isRunning = false;
            _windowTitleHelper = new WindowTitleHelper();
            _executionHelper = new ExecutionHelper(CanStart, Start);
            _keyInputInvoker = new Locator();
        }

        public void Initialize(Configuration config)
        {
            _playable = new TrackManager().CreateTracks(config.OwnedDlcs);

            _filter = config.FilterType switch
            {
                FilterType.Query => new QueryFilter(),
                FilterType.Playlist => new PlaylistFilter(),
                _ => throw new NotImplementedException(),
            };
            _filter.OutputMethod = new OutputMethodCreator().Create(config.Mode, config.Level);
            _candidates = _filter.Filter(_playable);

            _recent = new RecentHelper<string>(config.Exclusions, config.RecentsCount);
            SetCapacity(config.RecentsCount);
            _selector = new SelectorWithRecent(_recent);

            _keyInputInvoker.InputInterval = config.InputDelay;
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
                SetCapacity(_recent.Capacity);
            }
            var selected = _selector.Select(_candidates);
            if (selected is not null)
            {
                _keyInputInvoker?.Provide(selected, _playable.ToList());
                OnSelectionSuccessful.Invoke(selected);
            }
            else
            {
                ShowErrorMessageBox("There is no music that meets the filter conditions.");
            }
            _isRunning = false;
        }

        public void RegisterHook(Window window)
        {
            HwndSource source;
            IntPtr handle = new WindowInteropHelper(window).Handle;
            source = HwndSource.FromHwnd(handle);
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

        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            SetCapacity(message.Except);
            _filter.OutputMethod = new OutputMethodCreator().Create(message.Mode, message.Level);
            _keyInputInvoker.StartsAutomatically = message.Mode == MusicForm.Default && message.Aider == InputMethod.WithAutoStart;
            _keyInputInvoker.InvokesInput = message.Aider == InputMethod.NotInput;
            _executionHelper.IgnoreCanExecute = message.Aider == InputMethod.NotInput;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SettingMessage message, CancellationToken cancellationToken)
        {
            var outputMethod = _filter?.OutputMethod;
            _filter = message.FilterType switch
            {
                FilterType.Query => new QueryFilter(),
                FilterType.Playlist => new PlaylistFilter(),
                _ => throw new NotImplementedException(),
            };
            _filter.OutputMethod = outputMethod;
            _keyInputInvoker.InputInterval = message.InputInterval;
            _playable = new TrackManager().CreateTracks(message.OwnedDlcs);
            return Task.CompletedTask;
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SetCapacity(int value)
        {
            int titleCount = _candidates.Select(x => x.Title).Distinct().Count();
            int capacity = titleCount < value ? titleCount : value;
            _recent.Capacity = capacity;
        }
    }
}
