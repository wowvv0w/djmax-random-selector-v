using System;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Conditions
{
    public class ConditionBuilder : IConditionBuilder, IStateChangedNotifier<IFilterState>
    {
        public event StateChangedEventHandler<IFilterState> OnStateChanged;

        private IFilterState _filterState;

        public ICondition Build()
        {
            return _filterState.ToCondition();
        }

        public IFilterState GetState()
        {
            throw new NotImplementedException();
        }

        public void SetState(IFilterState state)
        {
            if (_filterState == state)
            {
                return;
            }
            _filterState = state;
            _filterState.OnStateChanged += () => OnStateChanged?.Invoke(null);
            OnStateChanged?.Invoke(null);
        }
    }
}
