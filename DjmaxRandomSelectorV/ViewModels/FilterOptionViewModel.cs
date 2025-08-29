using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
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
        public int ModeValue
        {
            get => (int)_filterOption.Mode;
            set
            {
                _filterOption.Mode = (MusicForm)value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ModeText));
            }
        }
        public int AiderValue
        {
            get => (int)_filterOption.Aider;
            set
            {
                _filterOption.Aider = (InputMethod)value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(AiderText));
            }
        }
        public int LevelValue
        {
            get => (int)_filterOption.Level;
            set
            {
                _filterOption.Level = (LevelPreference)value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(LevelText));
            }
        }
        public string ModeText { get => _modeItems[_filterOption.Mode]; }
        public string AiderText { get => _aiderItems[_filterOption.Aider]; }
        public string LevelText { get => _levelItems[_filterOption.Level]; }
        public int ModeMaxValue { get => Enum.GetValues<MusicForm>().Length - 1; }
        public int AiderMaxValue { get => Enum.GetValues<InputMethod>().Length - 1; }
        public int LevelMaxValue { get => Enum.GetValues<LevelPreference>().Length - 1; }

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
    }
}
