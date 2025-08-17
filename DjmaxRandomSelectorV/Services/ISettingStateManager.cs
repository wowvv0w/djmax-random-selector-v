using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Services
{
    public interface ISettingStateManager
    {
        event StateChangedEventHandler<ISettingState> OnSettingStateChanged;
        ISettingState GetSetting();
        void SetSetting(ISettingState setting);
    }
}
