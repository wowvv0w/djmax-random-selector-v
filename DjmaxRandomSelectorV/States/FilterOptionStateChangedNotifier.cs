using System;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.States
{
    internal class FilterOptionStateChangedNotifier : IStateChangedNotifier<IFilterOptionState>
    {
        public event StateChangedEventHandler<IFilterOptionState> OnStateChanged;

        private readonly IFilterOptionState _state;

        public FilterOptionStateChangedNotifier(IFilterOptionState state)
        {
            _state = new FilterOptionWrapper(state, () => OnStateChanged?.Invoke(state));
        }

        public IFilterOptionState GetState()
        {
            return _state;
        }

        public void SetState(IFilterOptionState state)
        {
            throw new NotImplementedException();
        }

        private class FilterOptionWrapper : IFilterOptionState
        {
            private readonly IFilterOptionState _state;
            private readonly Action _onStateChanged;
            public FilterOptionWrapper(IFilterOptionState state, Action onStateChanged)
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
    }
}
