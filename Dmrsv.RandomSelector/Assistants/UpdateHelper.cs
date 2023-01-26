namespace Dmrsv.RandomSelector
{
    public class UpdateHelper
    {
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
    
        public async Task<int[]> GetLastestVersionsAsync()
        {
            using var client = new HttpClient();
            string result = await client.GetStringAsync(VersionsUrl);
            string[] versions = result.Split(',');
            return Array.ConvertAll(versions, int.Parse);
        }
    }
}
