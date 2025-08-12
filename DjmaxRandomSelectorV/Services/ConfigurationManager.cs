using System;
using System.Collections.Generic;
using DjmaxRandomSelectorV.States;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public class ConfigurationManager : IFilterOptionStateManager, ISettingStateManager
    {
        public event StateChangedEventHandler<IFilterOptionState> OnFilterOptionStateChanged;
        public event StateChangedEventHandler<ISettingState> OnSettingStateChanged;

        private readonly Dmrsv3Configuration _config;

        public ConfigurationManager(Dmrsv3Configuration config)
        {
            _config = config;
        }

        public IFilterOptionState GetFilterOption()
        {
            return new FilterOptionStateWrapper(_config, () => OnFilterOptionStateChanged?.Invoke(_config));
        }

        public ISettingState GetSetting()
        {
            return new SettingStateCopy()
            {
                FilterType = _config.FilterType,
                InputDelay = _config.InputDelay,
                Favorite = new(_config.Favorite),
                Blacklist = new(_config.Blacklist),
                OwnedDlcs = new(_config.OwnedDlcs),
                SavesRecents = _config.SavesRecents,
                StartKeyCode = _config.StartKeyCode
            };
        }

        public void SetSetting(ISettingState state)
        {
            _config.FilterType = state.FilterType;
            _config.InputDelay = state.InputDelay;
            _config.Favorite = new(state.Favorite);
            _config.Blacklist = new(state.Blacklist);
            _config.OwnedDlcs = new(state.OwnedDlcs);
            _config.SavesRecents = state.SavesRecents;
            _config.StartKeyCode = state.StartKeyCode;
            OnSettingStateChanged?.Invoke(_config);
        }

        private class FilterOptionStateWrapper : IFilterOptionState
        {
            private readonly IFilterOptionState _state;
            private readonly Action _onStateChanged;
            public FilterOptionStateWrapper(IFilterOptionState state, Action onStateChanged)
            {
                _state = state;
                _onStateChanged = onStateChanged;
            }
            public int RecentsCount
            {
                get { return _state.RecentsCount; }
                set
                {
                    _state.RecentsCount = value;
                    _onStateChanged();
                }
            }
            public MusicForm Mode
            {
                get { return _state.Mode; }
                set
                {
                    _state.Mode = value;
                    _onStateChanged();
                }
            }
            public InputMethod Aider
            {
                get { return _state.Aider; }
                set
                {
                    _state.Aider = value;
                    _onStateChanged();
                }
            }
            public LevelPreference Level
            {
                get { return _state.Level; }
                set
                {
                    _state.Level = value;
                    _onStateChanged();
                }
            }
        }

        private class SettingStateCopy : ISettingState
        {
            public FilterType FilterType { get; set; }
            public int InputDelay { get; set; }
            public List<int> Favorite { get; set; }
            public List<int> Blacklist { get; set; }
            public List<string> OwnedDlcs { get; set; }
            public bool SavesRecents { get; set; }
            public uint StartKeyCode { get; set; }
        }

    }
}
