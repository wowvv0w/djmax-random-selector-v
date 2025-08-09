namespace DjmaxRandomSelectorV.States
{
    public interface IStateChangedNotifier<TState>
    {
        event StateChangedEventHandler<TState> OnStateChanged;
        TState GetState();
        void SetState(TState state);
    }
}
