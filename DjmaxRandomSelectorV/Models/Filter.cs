using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DjmaxRandomSelectorV.Models
{
    public class Filter
    {
        #region Fields
        private List<string> buttonTunes;
        private List<string> difficulties;
        private List<string> categories;
        private int[] levels;
        private List<string> recents;
        private bool includesFavorite;
        private bool isUpdated;
        #endregion

        #region Properties
        public List<string> ButtonTunes
        {
            get { return buttonTunes; }
            set { buttonTunes = value; }
        }
        public List<string> Difficulties
        {
            get { return difficulties; }
            set { difficulties = value; }
        }
        public List<string> Categories
        {
            get { return categories; }
            set { categories = value; }
        }
        public int[] Levels
        {
            get { return levels; }
            set { levels = value; }
        }
        public List<string> Recents
        {
            get { return recents; }
            set { recents = value; }
        }
        public bool IncludesFavorite
        {
            get { return includesFavorite; }
            set { includesFavorite = value; }
        }
        [JsonIgnore]
        public bool IsUpdated
        {
            get { return isUpdated; }
            set { isUpdated = value; }
        }
        #endregion

        #region Constants
        private const string CurrentFilterPath = "Data/CurrentFilter.json";
        #endregion

        public Filter()
        {
            isUpdated = true;
        }

        #region Lambda Expressions
        private readonly Func<string, string> presetPath = name => $"Data/Preset/{name}.json";
        #endregion

        #region Methods
        public void UpdateRecents(int titleCount, int maxCount)
        {
            int recentsCount = recents.Count;
            if (recentsCount > maxCount)
            {
                recents.RemoveRange(0, recentsCount - maxCount);
            }
            else if (recentsCount >= titleCount)
            {
                try
                {
                    recents.RemoveRange(0, recentsCount - titleCount + 1);
                }
                catch (ArgumentException)
                {
                }
            }
        }
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
