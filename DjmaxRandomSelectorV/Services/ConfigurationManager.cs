using System;
using System.Collections.Generic;
using System.Reflection;
using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Services
{
    public class ConfigurationManager : IFilterOptionStateManager, ISettingStateManager, IVersionInfoStateManager, IReadOnlyVersionInfoStateManager
    {
        public event StateChangedEventHandler<IFilterOptionState> OnFilterOptionStateChanged;
        public event StateChangedEventHandler<ISettingState> OnSettingStateChanged;

        private readonly Dmrsv3Configuration _config;

        private readonly FilterOptionStateWrapper _filterOption;
        private readonly VersionInfoWrapper _versionInfo;

        public ConfigurationManager(Dmrsv3Configuration config)
        {
            _config = config;
            _filterOption = new FilterOptionStateWrapper(_config, () => OnFilterOptionStateChanged?.Invoke(_filterOption));
            _versionInfo = new VersionInfoWrapper(_config);
        }

        public IFilterOptionState GetFilterOption()
        {
            return _filterOption;
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

        public void SetSetting(ISettingState setting)
        {
            _config.FilterType = setting.FilterType;
            _config.InputDelay = setting.InputDelay;
            _config.Favorite = new(setting.Favorite);
            _config.Blacklist = new(setting.Blacklist);
            _config.OwnedDlcs = new(setting.OwnedDlcs);
            _config.SavesRecents = setting.SavesRecents;
            _config.StartKeyCode = setting.StartKeyCode;
            OnSettingStateChanged?.Invoke(setting);
        }

        public IVersionInfoState GetVersionInfo()
        {
            return _versionInfo;
        }

        public IReadOnlyVersionInfoState GetReadOnlyVersionInfo()
        {
            return _versionInfo;
        }

        private class FilterOptionStateWrapper : IFilterOptionState
        {
            private readonly Dmrsv3Configuration _config;
            private readonly Action _onStateChanged;
            public FilterOptionStateWrapper(Dmrsv3Configuration config, Action onStateChanged)
            {
                _config = config;
                _onStateChanged = onStateChanged;
            }
            public int RecentsCount
            {
                get { return _config.RecentsCount; }
                set
                {
                    _config.RecentsCount = value;
                    _onStateChanged();
                }
            }
            public MusicForm Mode
            {
                get { return _config.Mode; }
                set
                {
                    _config.Mode = value;
                    _onStateChanged();
                }
            }
            public InputMethod Aider
            {
                get { return _config.Aider; }
                set
                {
                    _config.Aider = value;
                    _onStateChanged();
                }
            }
            public LevelPreference Level
            {
                get { return _config.Level; }
                set
                {
                    _config.Level = value;
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

        private class VersionInfoWrapper : IVersionInfoState, IReadOnlyVersionInfoState
        {
            private readonly Dmrsv3Configuration _config;
            public VersionInfoWrapper(Dmrsv3Configuration config)
            {
                _config = config;
                Version appVersion = Assembly.GetEntryAssembly().GetName().Version;
                CurrentAppVersion = appVersion;
                LatestAppVersion = appVersion;
                AllTrackVersion = config.AllTrackVersion;
                AppdataVersion = config.AppdataVersion;
            }
            public Version CurrentAppVersion { get; set; }
            public Version LatestAppVersion { get; set; }
            public long AllTrackVersion
            {
                get { return _config.AllTrackVersion; }
                set { _config.AllTrackVersion = value; }
            }
            public string AppdataVersion
            {
                get { return _config.AppdataVersion; }
                set { _config.AppdataVersion = value; }
            }
        }
    }
}
