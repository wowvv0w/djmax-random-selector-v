using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Services
{
    public interface IUpdateDownloader
    {
        Task<string[]> CheckUpdatesAsync();
        bool ExistsAppdataFile();
        bool ExistsAllTrackFile();
        Task DownloadAppdataAsync();
        Task DownloadAllTrackAsync();
    }
}
