using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterOptionIndicatorViewModel : Screen, IHandle<FilterOptionMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private int _exceptCount;
        private BitmapImage _modeImage;
        private BitmapImage _aiderImage;
        private BitmapImage _levelImage;

        public int ExceptCount
        {
            get { return _exceptCount; }
            set
            {
                _exceptCount = value;
                NotifyOfPropertyChange();
            }
        }
        public BitmapImage ModeImage
        {
            get { return _modeImage; }
            set
            {
                _modeImage = value;
                NotifyOfPropertyChange();
            }
        }
        public BitmapImage AiderImage
        {
            get { return _aiderImage; }
            set
            {
                _aiderImage = value;
                NotifyOfPropertyChange();
            }
        }
        public BitmapImage LevelImage
        {
            get { return _levelImage; }
            set
            {
                _levelImage = value;
                NotifyOfPropertyChange();
            }
        }

        public FilterOptionIndicatorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);

            var config = IoC.Get<Configuration>();
            SetExceptCount(config.RecentsCount);
            SetModeImage(config.Mode);
            SetAiderImage(config.Mode, config.Aider);
            SetLevelImage(config.Mode, config.Level);
        }

        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            SetExceptCount(message.RecentsCount);
            SetModeImage(message.MusicForm);
            SetAiderImage(message.MusicForm, message.InputMethod);
            SetLevelImage(message.MusicForm, message.LevelPreference);
            return Task.CompletedTask;
        }

        private void SetExceptCount(int value)
        {
            ExceptCount = value;
        }

        private void SetModeImage(MusicForm value)
        {
            ModeImage = GetBitmapImage(value switch
            {
                MusicForm.Default => "mode_fs",
                MusicForm.Free => "mode_on",
                _ => throw new NotImplementedException(),
            });
        }

        private void SetAiderImage(MusicForm mValue, InputMethod aValue)
        {
            AiderImage = GetBitmapImage((mValue, aValue) switch
            {
                (_, InputMethod.Default) => "addon_none",
                (MusicForm.Default, InputMethod.WithAutoStart) => "aider_auto",
                (MusicForm.Free, InputMethod.WithAutoStart) => "aider_auto_locked",
                (_, InputMethod.NotInput) => "aider_observe",
                _ => throw new NotImplementedException(),
            });
        }

        private void SetLevelImage(MusicForm mValue, LevelPreference lValue)
        {
            LevelImage = GetBitmapImage((mValue, lValue) switch
            {
                (_, LevelPreference.None) => "addon_none",
                (MusicForm.Default, LevelPreference.Lowest) => "level_beginner",
                (MusicForm.Default, LevelPreference.Highest) => "level_master",
                (MusicForm.Free, LevelPreference.Lowest) => "level_beginner_locked",
                (MusicForm.Free, LevelPreference.Highest) => "level_master_locked",
                _ => throw new NotImplementedException(),
            });
        }

        private BitmapImage GetBitmapImage(string fileName)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}.png"));
        }
    }
}
