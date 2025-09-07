using System;

namespace DjmaxRandomSelectorV.Services.Internal
{
    public interface IExecutable
    {
        event Action<bool> OnExecutionCompleted;
        bool IsRunning { get; }
        void Start();
        void Restart();
    }
}
