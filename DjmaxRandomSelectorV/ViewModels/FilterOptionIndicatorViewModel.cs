using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterOptionIndicatorViewModel : Screen, IHandle<FilterOption>
    {
        #region Filter Option Elements
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
                NotifyOfPropertyChange(() => ExceptCount);
            }
        }
        public BitmapImage ModeImage
        {
            get { return _modeImage; }
            set
            {
                _modeImage = value;
                NotifyOfPropertyChange(() => ModeImage);
            }
        }
        public BitmapImage AiderImage
        {
            get { return _aiderImage; }
            set
            {
                _aiderImage = value;
                NotifyOfPropertyChange(() => AiderImage);
            }
        }
        public BitmapImage LevelImage
        {
            get { return _levelImage; }
            set
            {
                _levelImage = value;
                NotifyOfPropertyChange(() => LevelImage);
            }
        }
        #endregion

        private IEventAggregator _eventAggregator;
        public FilterOptionIndicatorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        #region Image Modification
        public Task HandleAsync(FilterOption message, CancellationToken cancellationToken)
        {
            bool isFreestyle = message.Mode == Mode.Freestyle;

            ExceptCount = message.Except;
            SetBitmapImage(message.Mode);
            SetBitmapImage(message.Aider, isFreestyle);
            SetBitmapImage(message.Level, isFreestyle);

            return Task.CompletedTask;
        }
        private BitmapImage GetBitmapImage(string file)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/{file}.png"));
        }
        private void SetBitmapImage(Mode mode)
        {
            switch (mode)
            {
                case Mode.Freestyle:
                    ModeImage = GetBitmapImage("mode_fs");
                    break;
                case Mode.Online:
                    ModeImage = GetBitmapImage("mode_on");
                    break;
            }
        }
        private void SetBitmapImage(Aider aider, bool isFreestyle)
        {
            switch (aider)
            {
                case Aider.Off:
                    AiderImage = GetBitmapImage("addon_none");
                    break;
                case Aider.AutoStart:
                    AiderImage = GetBitmapImage(isFreestyle ? "aider_auto" : "aider_auto_locked");
                    break;
                case Aider.Observe:
                    AiderImage = GetBitmapImage("aider_observe");
                    break;
            }
        }
        private void SetBitmapImage(Level level, bool isFreestyle)
        {
            switch (level)
            {
                case Level.Off:
                    LevelImage = GetBitmapImage("addon_none");
                    break;
                case Level.Beginner:
                    LevelImage = GetBitmapImage(isFreestyle ? "level_beginner" : "level_beginner_locked");
                    break;
                case Level.Master:
                    LevelImage = GetBitmapImage(isFreestyle ? "level_master" : "level_master_locked");
                    break;
            }
        }
        #endregion
    }
}
