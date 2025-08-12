using DjmaxRandomSelectorV.Conditions;

namespace DjmaxRandomSelectorV.States
{
    public interface IFilterState
    {
        event StateChangedEventHandler OnStateChanged;
        ICondition ToCondition();
    }
}
