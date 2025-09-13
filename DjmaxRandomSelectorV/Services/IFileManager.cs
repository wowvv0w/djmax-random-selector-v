using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Services
{
    public interface IFileManager
    {
        T Load<T>(bool useDialog = false);
        void Save<T>(T instance, bool useDialog = false);
        Task DownloadAsync<T>();
        Task<string[]> CheckUpdatesAsync();
        bool Exists<T>();
    }
}
