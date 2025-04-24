using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV
{
    public interface IFileManager
    {
        T Import<T>(string path);
        void Export<T>(T instance, string path);
        Task<string> RequestAsync(string url);
        void Write(string content, string path);
    }

    public class FileManager : IFileManager
    {
        public T Import<T>(string path)
        {
            using var reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            T instance = JsonSerializer.Deserialize<T>(json, option);

            return instance ?? throw new NullReferenceException();
        }

        public void Export<T>(T instance, string path)
        {
            var option = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            string json = JsonSerializer.Serialize(instance, option);
            Write(json, path);
        }

        public async Task<string> RequestAsync(string url)
        {
            using var client = new HttpClient();
            string result = await client.GetStringAsync(url);
            return result;
        }

        public void Write(string content, string path)
        {
            using var writer = new StreamWriter(path);
            writer.Write(content);
        }
    }
}
