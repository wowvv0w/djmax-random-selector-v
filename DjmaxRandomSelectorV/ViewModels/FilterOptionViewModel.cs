using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.Models;
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

        private Config _config;
        public FilterOptionIndicatorViewModel FilterOptionIndicatorViewModel { get; set; }

        public FilterOptionViewModel(Config config)
        {
            _config = config;
            FilterOptionIndicatorViewModel = new FilterOptionIndicatorViewModel();
            _config.Subscribe(FilterOptionIndicatorViewModel);

            SetAddonText(_config.Mode);
            SetAddonText(_config.Aider);
            SetAddonText(_config.Level);
        }

        public int ExceptCount
        {
            get { return _config.Except; }
            set
            {
                _config.Except = value;
                NotifyOfPropertyChange(() => ExceptCount);
            }
        }

        private string modeText;
        public string ModeText
        {
            get { return modeText; }
            set
            {
                modeText = value;
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
        public void SwitchMode()
        {
            if (_config.Mode.Equals(Mode.Freestyle))
                _config.Mode = Mode.Online;
            else
                _config.Mode = Mode.Freestyle;
            SetAddonText(_config.Mode);
        }

        private string aiderText;
        public string AiderText
        {
            get { return aiderText; }
            set
            {
                aiderText = value;
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
        public void PrevAider()
        {
            if (_config.Aider.Equals(Aider.Off))
                _config.Aider = Aider.Observe;
            else
                _config.Aider--;
            SetAddonText(_config.Aider);
        }
        public void NextAider()
        {
            if (_config.Aider.Equals(Aider.Observe))
                _config.Aider = Aider.Off;
            else
                _config.Aider++;
            SetAddonText(_config.Aider);
        }

        private string levelText;
        public string LevelText
        {
            get { return levelText; }
            set
            {
                levelText = value;
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
        public void PrevLevel()
        {
            if (_config.Level.Equals(Level.Off))
                _config.Level = Level.Master;
            else
                _config.Level--;
            SetAddonText(_config.Level);
        }
        public void NextLevel()
        {
            if (_config.Level.Equals(Level.Master))
                _config.Level = Level.Off;
            else
                _config.Level++;
            SetAddonText(_config.Level);
        }
    }
}
