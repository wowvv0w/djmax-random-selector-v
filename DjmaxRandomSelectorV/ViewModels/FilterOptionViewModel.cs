using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterOptionViewModel : Conductor<object>
    {
        private readonly Dictionary<MusicForm, string> _modeItems = new()
        {
            [MusicForm.Default] = "FREESTYLE",
            [MusicForm.Free] = "ONLINE",
        };
        private readonly Dictionary<InputMethod, string> _aiderItems = new()
        {
            [InputMethod.Default] = "OFF",
            [InputMethod.WithAutoStart] = "AUTO START",
            [InputMethod.NotInput] = "OBSERVE",
        };
        private readonly Dictionary<LevelPreference, string> _levelItems = new()
        {
            [LevelPreference.None] = "OFF",
            [LevelPreference.Lowest] = "BEGINNER",
            [LevelPreference.Highest] = "MASTER",
        };
        private readonly IEventAggregator _eventAggregator;
        private readonly IFilterOptionStateManager _filterOptionManager;
        private readonly IFilterOptionState _filterOption;

        public object FilterOptionIndicator { get => ActiveItem; }
        public int ExceptCount
        {
            get => _filterOption.RecentsCount;
            set
            {
                _filterOption.RecentsCount = value;
                NotifyOfPropertyChange();
            }
        }
        public string ModeText { get => _modeItems[_filterOption.Mode]; }
        public string AiderText { get => _aiderItems[_filterOption.Aider]; }
        public string LevelText { get => _levelItems[_filterOption.Level]; }

        public FilterOptionViewModel(IEventAggregator eventAggregator, IFilterOptionStateManager filterOptionManager)
        {
            _eventAggregator = eventAggregator;
            _filterOptionManager = filterOptionManager;
            _filterOption = _filterOptionManager.GetFilterOption();
            _filterOptionManager.OnFilterOptionStateChanged += PublishMessage;
            ActivateItemAsync(IoC.Get<FilterOptionIndicatorViewModel>());
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                _filterOptionManager.OnFilterOptionStateChanged -= PublishMessage;
            }
            return Task.CompletedTask;
        }

        private void PublishMessage(IFilterOptionState filterOption)
        {
            _eventAggregator.PublishOnUIThreadAsync(new FilterOptionMessage(filterOption));
        }

        public void SwitchMode()
        {
            int value = (int)_filterOption.Mode;
            value ^= 0x1;
            _filterOption.Mode = (MusicForm)value;
            NotifyOfPropertyChange(nameof(ModeText));
        }

        public void SwitchAider(int move)
        {
            int value = (int)_filterOption.Aider;
            value += move;
            value = (value % 3 + 3) % 3;
            _filterOption.Aider = (InputMethod)value;
            NotifyOfPropertyChange(nameof(AiderText));
        }

        public void SwitchLevel(int move)
        {
            int value = (int)_filterOption.Level;
            value += move;
            value = (value % 3 + 3) % 3;
            _filterOption.Level = (LevelPreference)value;
            NotifyOfPropertyChange(nameof(LevelText));
        }
    }
}
