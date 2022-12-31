using Caliburn.Micro;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.Controller;
using Dmrsv.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterOptionViewModel : Screen
    {
        private const string OFF = "OFF";
        private const string FREESTYLE = "FREESTYLE";
        private const string ONLINE = "ONLINE";
        private const string AUTO_START = "AUTO START";
        private const string OBSERVE = "OBSERVE";
        private const string BEGINNER = "BEGINNER";
        private const string MASTER = "MASTER";

        private readonly IEventAggregator _eventAggregator;
        private readonly FilterOption _filterOption;
        private readonly OptionApi _api;

        public FilterOptionIndicatorViewModel FilterOptionIndicator { get; set; }

        public FilterOptionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _api = new OptionApi();
            _filterOption = _api.GetFilterOption();

            FilterOptionIndicator = IoC.Get<FilterOptionIndicatorViewModel>();

            SetAddonText(_filterOption.Mode);
            SetAddonText(_filterOption.Aider);
            SetAddonText(_filterOption.Level);

            Publish();
        }

        private void Publish()
        {
            _api.SetFilterOption(_filterOption);
            _eventAggregator.PublishOnUIThreadAsync(_filterOption);
        }

        public int ExceptCount
        {
            get { return _filterOption.Except; }
            set
            {
                _filterOption.Except = value;
                NotifyOfPropertyChange(() => ExceptCount);
                Publish();
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

        public void SwitchMode()
        {
            if (_filterOption.Mode == Mode.Freestyle)
                _filterOption.Mode = Mode.Online;
            else
                _filterOption.Mode = Mode.Freestyle;
            SetAddonText(_filterOption.Mode);
            Publish();
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

        public void PrevAider()
        {
            if (_filterOption.Aider == Aider.Off)
                _filterOption.Aider = Aider.Observe;
            else
                _filterOption.Aider--;
            SetAddonText(_filterOption.Aider);
            Publish();
        }
        public void NextAider()
        {
            if (_filterOption.Aider == Aider.Observe)
                _filterOption.Aider = Aider.Off;
            else
                _filterOption.Aider++;
            SetAddonText(_filterOption.Aider);
            Publish();
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

        public void PrevLevel()
        {
            if (_filterOption.Level == Level.Off)
                _filterOption.Level = Level.Master;
            else
                _filterOption.Level--;
            SetAddonText(_filterOption.Level);
            Publish();
        }
        public void NextLevel()
        {
            if (_filterOption.Level == Level.Master)
                _filterOption.Level = Level.Off;
            else
                _filterOption.Level++;
            SetAddonText(_filterOption.Level);
            Publish();
        }
    }
}
