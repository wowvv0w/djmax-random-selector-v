using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterOptionViewModel : Screen
    {
        #region Constants
        private const string OFF = "OFF";
        private const string FREESTYLE = "FREESTYLE";
        private const string ONLINE = "ONLINE";
        private const string AUTO_START = "AUTO START";
        private const string OBSERVE = "OBSERVE";
        private const string BEGINNER = "BEGINNER";
        private const string MASTER = "MASTER";
        #endregion        

        public FilterOptionIndicatorViewModel FilterOptionIndicatorViewModel { get; set; }

        public FilterOptionViewModel()
        {
            FilterOption filterOption = FileManager.Import<Config>("Data/Config.json").FilterOption;
            FilterOptionIndicatorViewModel = new FilterOptionIndicatorViewModel();

            SetAddonText(_mode = filterOption.Mode);
            SetAddonText(_aider = filterOption.Aider);
            SetAddonText(_level = filterOption.Level);
        }

        private int _exceptCount;
        public int ExceptCount
        {
            get { return _exceptCount; }
            set
            {
                _exceptCount = value;
                NotifyOfPropertyChange(() => ExceptCount);
            }
        }

        private string _modeText;
        public string ModeText
        {
            get { return _modeText; }
            set
            {
                _modeText = value;
                NotifyOfPropertyChange(() => ModeText);
            }
        }
        private void SetAddonText(Mode mode)
        {
            switch (mode)
            {
                case Mode.Freestyle:
                    ModeText = FREESTYLE;
                    break;
                case Mode.Online:
                    ModeText = ONLINE;
                    break;
            }
        }

        private Mode _mode;
        public void SwitchMode()
        {
            if (_mode.Equals(Mode.Freestyle))
                _mode = Mode.Online;
            else
                _mode = Mode.Freestyle;
            SetAddonText(_mode);
        }

        private string _aiderText;
        public string AiderText
        {
            get { return _aiderText; }
            set
            {
                _aiderText = value;
                NotifyOfPropertyChange(() => AiderText);
            }
        }
        private void SetAddonText(Aider aider)
        {
            switch (aider)
            {
                case Aider.Off:
                    AiderText = OFF;
                    break;
                case Aider.AutoStart:
                    AiderText = AUTO_START;
                    break;
                case Aider.Observe:
                    AiderText = OBSERVE;
                    break;
            }
        }

        private Aider _aider;
        public void PrevAider()
        {
            if (_aider.Equals(Aider.Off))
                _aider = Aider.Observe;
            else
                _aider--;
            SetAddonText(_aider);
        }
        public void NextAider()
        {
            if (_aider.Equals(Aider.Observe))
                _aider = Aider.Off;
            else
                _aider++;
            SetAddonText(_aider);
        }

        private string _levelText;
        public string LevelText
        {
            get { return _levelText; }
            set
            {
                _levelText = value;
                NotifyOfPropertyChange(() => LevelText);
            }
        }
        private void SetAddonText(Level level)
        {
            switch (level)
            {
                case Level.Off:
                    LevelText = OFF;
                    break;
                case Level.Beginner:
                    LevelText = BEGINNER;
                    break;
                case Level.Master:
                    LevelText = MASTER;
                    break;
            }
        }

        private Level _level;
        public void PrevLevel()
        {
            if (_level.Equals(Level.Off))
                _level = Level.Master;
            else
                _level--;
            SetAddonText(_level);
        }
        public void NextLevel()
        {
            if (_level.Equals(Level.Master))
                _level = Level.Off;
            else
                _level++;
            SetAddonText(_level);
        }
    }
}
