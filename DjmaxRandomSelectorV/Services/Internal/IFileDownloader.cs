using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Services.Internal
{
    public interface IFileDownloader
    {
        Task<string[]> CheckUpdatesAsync();
        bool ExistsAppdataFile();
        bool ExistsAllTrackFile();
        Task DownloadAppdataAsync();
        Task DownloadAllTrackAsync();
    }
}
