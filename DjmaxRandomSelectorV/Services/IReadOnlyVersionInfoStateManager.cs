using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.Services
{
    public interface IReadOnlyVersionInfoStateManager
    {
        IReadOnlyVersionInfoState GetReadOnlyVersionInfo();
    }
}
