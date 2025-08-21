using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.ViewModels
{
    public partial class FilterOptionViewModel : ObservableObject
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
        //private readonly IEventAggregator _eventAggregator;
        private readonly IFilterOptionStateManager _filterOptionManager;
        private readonly IFilterOptionState _filterOption;

        public object FilterOptionIndicator { get; }
        public int ExceptCount
        {
            get => _filterOption.RecentsCount;
            set => SetProperty(_filterOption.RecentsCount, value, _filterOption, (fo, val) => fo.RecentsCount = val);
        }
        [ObservableProperty]
        private string _modeText;
        [ObservableProperty]
        private string _aiderText;
        [ObservableProperty]
        private string _levelText;
        //public string ModeText { get => _modeItems[_filterOption.Mode]; }
        //public string AiderText { get => _aiderItems[_filterOption.Aider]; }
        //public string LevelText { get => _levelItems[_filterOption.Level]; }

        public FilterOptionViewModel(IFilterOptionStateManager filterOptionManager)
        {
            //_eventAggregator = eventAggregator;
            _filterOptionManager = filterOptionManager;
            _filterOption = _filterOptionManager.GetFilterOption();
            _filterOptionManager.OnFilterOptionStateChanged += PublishMessage;
            //FilterOptionIndicator = Ioc.Default.GetRequiredService<FilterOptionIndicatorViewModel>();

            _modeText = _modeItems[_filterOption.Mode];
            _aiderText = _aiderItems[_filterOption.Aider];
            _levelText = _levelItems[_filterOption.Level];
        }

        //protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        //{
        //    if (close)
        //    {
        //        _filterOptionManager.OnFilterOptionStateChanged -= PublishMessage;
        //    }
        //    return Task.CompletedTask;
        //}

        private void PublishMessage(IFilterOptionState filterOption)
        {
            //_eventAggregator.PublishOnUIThreadAsync(new FilterOptionMessage(filterOption));
        }

        [RelayCommand]
        private void SwitchMode()
        {
            int value = (int)_filterOption.Mode;
            value ^= 0x1;
            _filterOption.Mode = (MusicForm)value;
            ModeText = _modeItems[_filterOption.Mode];
        }

        [RelayCommand]
        private void SwitchAider(string move)
        {
            int value = (int)_filterOption.Aider;
            value += move == "prev" ? -1 : 1;
            value = (value % 3 + 3) % 3;
            _filterOption.Aider = (InputMethod)value;
            AiderText = _aiderItems[_filterOption.Aider];
        }

        [RelayCommand]
        private void SwitchLevel(string move)
        {
            int value = (int)_filterOption.Level;
            value += move == "prev" ? -1 : 1;
            value = (value % 3 + 3) % 3;
            _filterOption.Level = (LevelPreference)value;
            LevelText = _levelItems[_filterOption.Level];
        }
    }
}
