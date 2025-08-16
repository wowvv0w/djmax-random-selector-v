using System;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Services
{
    public class ConditionManager : IConditionBuilder, IFilterStateManager
    {
        public event Action OnFilterStateChanged;

        private IFilterState _filterState;

        public ICondition Build()
        {
            return _filterState.ToCondition();
        }

        public void RegisterFilterState(IFilterState filter)
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
