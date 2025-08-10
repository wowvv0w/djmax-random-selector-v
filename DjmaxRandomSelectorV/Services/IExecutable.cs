namespace DjmaxRandomSelectorV.Services
{
    public interface IExecutable
    {
        bool IsRunning { get; }
        void Start();
        void Restart();
    }
}
