using System;
using System.IO;
using System.Text.Json;

namespace DjmaxRandomSelectorV
{
    public interface IFileManager
    {
        T Import<T>(string path);
        void Export<T>(T instance, string path);
    }

    public class FileManager : IFileManager
    {
        public T Import<T>(string path)
        {
            using var reader = new StreamReader(path);

            string json = reader.ReadToEnd();
            T instance = JsonSerializer.Deserialize<T>(json);

            return instance ?? throw new NullReferenceException();
        }

        public void Export<T>(T instance, string path)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreReadOnlyProperties = false
            };

            string jsonString = JsonSerializer.Serialize(instance, options);

            using var writer = new StreamWriter(path);
            writer.Write(jsonString);
        }
    }
}
