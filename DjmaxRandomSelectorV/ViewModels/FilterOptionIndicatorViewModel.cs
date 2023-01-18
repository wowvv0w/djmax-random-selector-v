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
        }

        public Task HandleAsync(FilterOptionMessage message, CancellationToken cancellationToken)
        {
            ExceptCount = message.Except;
            MusicForm mode = message.Mode;
            ModeImage = GetBitmapImage(mode switch
            {
                MusicForm.Default => "mode_fs",
                MusicForm.Free => "mode_on",
                _ => throw new NotImplementedException(),
            });
            AiderImage = GetBitmapImage((mode, message.Aider) switch
            {
                (_, InputMethod.Default) => "addon_none",
                (MusicForm.Default, InputMethod.WithAutoStart) => "aider_auto",
                (MusicForm.Free, InputMethod.WithAutoStart) => "aider_auto_locked",
                (_, InputMethod.NotInput) => "aider_observe",
                _ => throw new NotImplementedException(),
            });
            LevelImage = GetBitmapImage((mode, message.Level) switch
            {
                (_, LevelPreference.None) => "addon_none",
                (MusicForm.Default, LevelPreference.Lowest) => "level_beginner",
                (MusicForm.Free, LevelPreference.Lowest) => "level_beginner_locked",
                (MusicForm.Default, LevelPreference.Highest) => "level_master",
                (MusicForm.Free, LevelPreference.Highest) => "level_master_locked",
                _ => throw new NotImplementedException(),
            });
            return Task.CompletedTask;
        }

        private BitmapImage GetBitmapImage(string fileName)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}.png"));
        }
    }
}
