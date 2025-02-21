using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        private int _except;
        private MusicForm _mode;
        private InputMethod _aider;
        private LevelPreference _level;

        public object FilterOptionIndicator { get => ActiveItem; }
        public int ExceptCount
        {
            get => _except;
            set
            {
                _except = value;
                NotifyOfPropertyChange();
                Publish(new FilterOptionMessage(_except, _mode, _aider, _level));
            }
        }
        public string ModeText { get => _modeItems[_mode]; }
        public string AiderText { get => _aiderItems[_aider]; }
        public string LevelText { get => _levelItems[_level]; }

        public FilterOptionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            var filterOption = IoC.Get<Dmrsv3Configuration>().FilterOption;
            _except = filterOption.RecentsCount;
            _mode = filterOption.MusicForm;
            _aider = filterOption.InputMethod;
            _level = filterOption.LevelPreference;
            ActivateItemAsync(IoC.Get<FilterOptionIndicatorViewModel>());
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                var filterOption = IoC.Get<Dmrsv3Configuration>().FilterOption;
                filterOption.RecentsCount = _except;
                filterOption.MusicForm = _mode;
                filterOption.InputMethod = _aider;
                filterOption.LevelPreference = _level;
            }
            return Task.CompletedTask;
        }

        private void Publish(object message)
        {
            _eventAggregator.PublishOnUIThreadAsync(message);
        }

        public void SwitchMode()
        {
            int value = (int)_mode;
            value ^= 0x1;
            _mode = (MusicForm)value;
            NotifyOfPropertyChange(nameof(ModeText));
            Publish(new FilterOptionMessage(_except, _mode, _aider, _level));
        }

        public void SwitchAider(int move)
        {
            int value = (int)_aider;
            value += move;
            value = (value % 3 + 3) % 3;
            _aider = (InputMethod)value;
            NotifyOfPropertyChange(nameof(AiderText));
            Publish(new FilterOptionMessage(_except, _mode, _aider, _level));
        }

        public void SwitchLevel(int move)
        {
            int value = (int)_level;
            value += move;
            value = (value % 3 + 3) % 3;
            _level = (LevelPreference)value;
            NotifyOfPropertyChange(nameof(LevelText));
            Publish(new FilterOptionMessage(_except, _mode, _aider, _level));
        }
    }
}
