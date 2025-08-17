using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Services
{
    public interface IFilterOptionStateManager
    {
        event StateChangedEventHandler<IFilterOptionState> OnFilterOptionStateChanged;
        IFilterOptionState GetFilterOption();
    }
}
