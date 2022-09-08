using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterOptionIndicatorViewModel : Screen
    {
        #region Filter Option Elements
        private int exceptCount;
        private BitmapImage modeImage;
        private BitmapImage aiderImage;
        private BitmapImage levelImage;

        public int ExceptCount
        {
            get { return exceptCount; }
            set
            {
                exceptCount = value;
                NotifyOfPropertyChange(() => ExceptCount);
            }
        }
        public BitmapImage ModeImage
        {
            get { return modeImage; }
            set
            {
                modeImage = value;
                NotifyOfPropertyChange(() => ModeImage);
            }
        }
        public BitmapImage AiderImage
        {
            get { return aiderImage; }
            set
            {
                aiderImage = value;
                NotifyOfPropertyChange(() => AiderImage);
            }
        }
        public BitmapImage LevelImage
        {
            get { return levelImage; }
            set
            {
                levelImage = value;
                NotifyOfPropertyChange(() => LevelImage);
            }
        }
        #endregion

        #region Image Modification
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
