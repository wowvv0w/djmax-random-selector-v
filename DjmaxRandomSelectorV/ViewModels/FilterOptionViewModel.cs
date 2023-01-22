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
        private readonly IEventAggregator _eventAggregator;
        private readonly FilterOptionMessage _message;
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

        public object FilterOptionIndicator { get => ActiveItem; }
        public int ExceptCount
        {
            get => _message.Except;
            set
            {
                _message.Except = value;
                NotifyOfPropertyChange();
                Publish();
            }
        }
        public string ModeText { get => _modeItems[_message.Mode]; }
        public string AiderText { get => _aiderItems[_message.Aider]; }
        public string LevelText { get => _levelItems[_message.Level]; }

        public FilterOptionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            var config = IoC.Get<Configuration>();
            _message = new FilterOptionMessage()
            {
                Except = config.RecentsCount,
                Mode = config.Mode,
                Aider = config.Aider,
                Level = config.Level,
            };
            ActivateItemAsync(IoC.Get<FilterOptionIndicatorViewModel>());
            Publish();
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                var config = IoC.Get<Configuration>();
                config.RecentsCount = _message.Except;
                config.Mode = _message.Mode;
                config.Aider = _message.Aider;
                config.Level = _message.Level;
            }
            return Task.CompletedTask;
        }

        private void Publish()
        {
            _eventAggregator.PublishOnUIThreadAsync(_message);
        }

        public void SwitchMode()
        {
            int value = (int)_message.Mode;
            value ^= 0x1;
            _message.Mode = (MusicForm)value;
            NotifyOfPropertyChange(nameof(ModeText));
            Publish();
        }

        public void SwitchAider(int move)
        {
            int value = (int)_message.Aider;
            value += move;
            value = (value % 3 + 3) % 3;
            _message.Aider = (InputMethod)value;
            NotifyOfPropertyChange(nameof(AiderText));
            Publish();
        }

        public void SwitchLevel(int move)
        {
            int value = (int)_message.Level;
            value += move;
            value = (value % 3 + 3) % 3;
            _message.Level = (LevelPreference)value;
            NotifyOfPropertyChange(nameof(LevelText));
            Publish();
        }
    }
}
