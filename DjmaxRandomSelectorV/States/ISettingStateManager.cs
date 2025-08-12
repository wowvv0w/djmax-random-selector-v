namespace DjmaxRandomSelectorV.States
{
    public interface ISettingStateManager
    {
        event StateChangedEventHandler<ISettingState> OnSettingStateChanged;
        ISettingState GetSetting();
        void SetSetting(ISettingState setting);
    }
}
