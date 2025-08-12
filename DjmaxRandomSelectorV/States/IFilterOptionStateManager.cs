namespace DjmaxRandomSelectorV.States
{
    public interface IFilterOptionStateManager
    {
        event StateChangedEventHandler<IFilterOptionState> OnFilterOptionStateChanged;
        IFilterOptionState GetFilterOption();
    }
}
