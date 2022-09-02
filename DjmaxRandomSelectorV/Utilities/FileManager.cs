using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Utilities
{
    public static class FileManager
    {
        public static T Import<T>(string path) where T : new()
        {
            try
            {
                using var reader = new StreamReader(path);

                string json = reader.ReadToEnd();
                T instance = JsonSerializer.Deserialize<T>(json);

                return instance;
            }
            catch (FileNotFoundException)
            {
                return new T();
            }
        }
        public static void Export<T>(T instance, string path)
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
