using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Services
{
    public interface IFilterStateManager
    {
        void RegisterFilterState(IFilterState filter);
    }
}
