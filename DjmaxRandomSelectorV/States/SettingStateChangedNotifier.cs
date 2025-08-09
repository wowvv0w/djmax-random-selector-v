using System;
using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.States
{
    public class SettingStateChangedNotifier : IStateChangedNotifier<ISettingState>
    {
        public event StateChangedEventHandler<ISettingState> OnStateChanged;

        private readonly ISettingState _state;

        public SettingStateChangedNotifier(ISettingState state)
        {
            _state = state;
        }
        public ISettingState GetState()
        {
            return new SettingWrapper()
            {
                FilterType = _state.FilterType,
                InputDelay = _state.InputDelay,
                Favorite = new(_state.Favorite),
                Blacklist = new(_state.Blacklist),
                OwnedDlcs = new(_state.OwnedDlcs),
                SavesRecents = _state.SavesRecents,
                StartKeyCode = _state.StartKeyCode
            };
        }

        public void SetState(ISettingState state)
        {
            _state.FilterType = state.FilterType;
            _state.InputDelay = state.InputDelay;
            _state.Favorite = new(state.Favorite);
            _state.Blacklist = new(state.Blacklist);
            _state.OwnedDlcs = new(state.OwnedDlcs);
            _state.SavesRecents = state.SavesRecents;
            _state.StartKeyCode = state.StartKeyCode;
            OnStateChanged?.Invoke(_state);
        }

        private class SettingWrapper : ISettingState
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
