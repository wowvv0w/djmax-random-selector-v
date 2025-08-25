using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Services;
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

        public object ExceptCountUpdater { get; }
        public object ModeUpdater { get; }
        public object AiderUpdater { get; }
        public object LevelUpdater { get; }

        public FilterOptionViewModel(IEventAggregator eventAggregator, IFilterOptionStateManager filterOptionManager)
        {
            _eventAggregator = eventAggregator;
            _filterOptionManager = filterOptionManager;
            _filterOption = _filterOptionManager.GetFilterOption();
            _filterOptionManager.OnFilterOptionStateChanged += PublishMessage;

            ExceptCountUpdater = new SettingSliderItem(
                "EXCLUDE RECENT MUSICS",
                0,
                30,
                1,
                () => _filterOption.RecentsCount,
                newValue => _filterOption.RecentsCount = newValue);

            ModeUpdater = new SettingSpinBoxItem(
                "MODE",
                () => (int)_filterOption.Mode,
                newValue => _filterOption.Mode = (MusicForm)newValue,
                (int)MusicForm.Default,
                (int)MusicForm.Free,
                true,
                value => _modeItems[(MusicForm)value]);

            AiderUpdater = new SettingSpinBoxItem(
                "AIDER",
                () => (int)_filterOption.Aider,
                newValue => _filterOption.Aider = (InputMethod)newValue,
                (int)InputMethod.Default,
                (int)InputMethod.NotInput,
                true,
                value => _aiderItems[(InputMethod)value]);

            LevelUpdater = new SettingSpinBoxItem(
                "LEVEL",
                () => (int)_filterOption.Level,
                newValue => _filterOption.Level = (LevelPreference)newValue,
                (int)LevelPreference.None,
                (int)LevelPreference.Highest,
                true,
                value => _levelItems[(LevelPreference)value]);

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
