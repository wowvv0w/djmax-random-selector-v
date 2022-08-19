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
        private int[] scLevels;
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
        public int[] ScLevels
        {
            get { return scLevels; }
            set { scLevels = value; }
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
        public void Import(string presetPath = null)
        {
            bool isNull = string.IsNullOrEmpty(presetPath);
            var path = isNull ? CurrentFilterPath : presetPath;
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
                    ScLevels = filter.ScLevels;
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
                    ScLevels = new int[2] { 1, 15 };
                    Recents = new List<string>();
                    IncludesFavorite = false;
                }
                else
                {
                    throw e;
                }
            }
        }
        public void Export(string presetPath = null)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreReadOnlyProperties = false
            };
            string jsonString = JsonSerializer.Serialize(this, options);
            var path = string.IsNullOrEmpty(presetPath) ? CurrentFilterPath : presetPath;

            using (var writer = new StreamWriter(path))
            { 
                writer.Write(jsonString);
            }
        }
        #endregion
    }
}
