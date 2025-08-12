using System;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Conditions
{
    public class ConditionBuilder : IConditionBuilder, IFilterStateManager
    {
        public event Action OnFilterStateChanged;

        private IFilterState _filterState;

        public ICondition Build()
        {
            return _filterState.ToCondition();
        }

        public void SetFilterState(IFilterState filter)
        {
            if (_filterState == filter)
            {
                return;
            }
            _filterState = filter;
            _filterState.OnStateChanged += () => OnFilterStateChanged?.Invoke();
            OnFilterStateChanged?.Invoke();
        }
    }
}
