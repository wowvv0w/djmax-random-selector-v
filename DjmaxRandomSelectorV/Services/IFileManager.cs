using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Services
{
    public interface IFileManager
    {
        T Load<T>(bool useDialog);
        void Save<T>(T instance, bool useDialog);
        Task DownloadAsync<T>();
        Task<string[]> CheckUpdatesAsync();
        bool Exists<T>();
    }
}
