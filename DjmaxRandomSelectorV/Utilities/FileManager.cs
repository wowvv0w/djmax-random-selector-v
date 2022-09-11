using CsvHelper;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Utilities
{
    public static class FileManager
    {
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";
        private const string AllTrackListPath = "Data/AllTrackList.csv";
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
        public static void DownloadAllTrackList()
        {
            using var client = new HttpClient();
            string result = client.GetStringAsync(AllTrackListUrl).Result;

            using var writer = new StreamWriter(AllTrackListPath);
            writer.Write(result);
        }
        public static IEnumerable<Track> GetAllTrackList()
        {
            using var reader = new StreamReader(AllTrackListPath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TrackMap>();
            var records = csv.GetRecords<Track>();

            return records.ToList();
        }
    }
}
