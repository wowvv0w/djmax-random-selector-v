using System;
using DjmaxRandomSelectorV.Conditions;

namespace DjmaxRandomSelectorV.States
{
    public interface IFilterState
    {
        event Action OnStateChanged;
        ICondition ToCondition();
    }
}
