using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DjmaxRandomSelectorV.Models
{
    public class Filter
    {
        #region Properties
        public List<string> ButtonTunes { get; set; }
        public List<string> Difficulties { get; set; }
        public List<string> Categories { get; set; }
        public int[] Levels { get; set; } // 1 ~ 15
        public List<string> Recents { get; set; }
        public bool IncludesFavorite { get; set; }
        #endregion

        #region Constants
        private const string CurrentFilterPath = "Data/CurrentFilter.json";
        #endregion

        #region Methods
        private readonly Func<string, string> presetPath = name => $"Data/Preset/{name}.json";
        public void Import(string presetName = null)
        {
            bool isNull = string.IsNullOrEmpty(presetName);
            var path = isNull ? CurrentFilterPath : presetPath(presetName);
            try
            {
                using (var reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    Filter filter = JsonSerializer.Deserialize<Filter>(json);

                    ButtonTunes = filter.ButtonTunes;
                    Difficulties = filter.Difficulties;
                    Categories = filter.Categories;
                    Levels = filter.Levels;
                    Recents = filter.Recents;
                    IncludesFavorite = filter.IncludesFavorite;
                }
            }
            catch (FileNotFoundException e)
            {
                if (isNull)
                {
                    ButtonTunes = new List<string>();
                    Difficulties = new List<string>();
                    Categories = new List<string>();
                    Levels = new int[2] { 1, 15 };
                    Recents = new List<string>();
                    IncludesFavorite = false;
                }
                else
                {
                    throw e;
                }
            }
        }

        public void Export(string presetName = null)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreReadOnlyProperties = false
            };
            string jsonString = JsonSerializer.Serialize(this, options);
            var path = string.IsNullOrEmpty(presetName) ? CurrentFilterPath : presetPath(presetName);

            using (var writer = new StreamWriter(path))
            { 
                writer.Write(jsonString);
            }
        }
        #endregion
    }
}
