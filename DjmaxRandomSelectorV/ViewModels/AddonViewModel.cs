using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class AddonViewModel : Screen
    {
        public AddonViewModel(Setting setting)
        {
            ExceptCount = setting.RecentsCount;
            SetBitmapImage(setting.Aider);
        }

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

        private BitmapImage GetBitmapImage(string file)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/{file}.png"));
        }

        public void SetBitmapImage(Mode mode)
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

        public void SetBitmapImage(Aider aider)
        {
            switch (aider)
            {
                case Aider.Off:
                    AiderImage = GetBitmapImage("addon_none");
                    break;
                case Aider.AutoStart:
                    AiderImage = GetBitmapImage("aider_auto");
                    break;
                case Aider.Observe:
                    AiderImage = GetBitmapImage("aider_observe");
                    break;
            }
        }

        public void SetBitmapImage(Level level)
        {
            switch (level)
            {
                case Level.Off:
                    LevelImage = GetBitmapImage("addon_none");
                    break;
                case Level.Beginner:
                    LevelImage = GetBitmapImage("level_beginner");
                    break;
                case Level.Standard:
                    LevelImage = GetBitmapImage("level_standard");
                    break;
                case Level.Master:
                    LevelImage = GetBitmapImage("level_master");
                    break;
            }
        }

    }
}
