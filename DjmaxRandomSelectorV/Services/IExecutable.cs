using System;

namespace DjmaxRandomSelectorV.Services
{
    public interface IExecutable
    {
        event Action<bool> OnExecutionCompleted;
        bool IsRunning { get; }
        void Start();
        void Restart();
    }
}
