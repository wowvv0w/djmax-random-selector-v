using System.Text.Json;

namespace Dmrsv.RandomSelector
{
    public interface IFileManager
    {
        T Import<T>(string path) where T : new();
        void Export<T>(T instance, string path);
    }

    public class FileManager : IFileManager
    {
        public T Import<T>(string path) where T : new()
        {
            try
            {
                using var reader = new StreamReader(path);

                string json = reader.ReadToEnd();
                T? instance = JsonSerializer.Deserialize<T>(json);

                return instance ?? new T();
            }
            catch (FileNotFoundException)
            {
                return new T();
            }
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
