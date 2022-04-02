using DjmaxRandomSelectorV.DataTypes.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DjmaxRandomSelectorV.Models
{
    public class Setting
    {
        #region Constants
        private const string CONFIG = "Data/Config.json";
        #endregion

        #region Properties
        public int RecentsCount { get; set; }
        public Mode Mode { get; set; }
        public Aider Aider { get; set; }
        public Level Level { get; set; }
        public int InputDelay { get; set; }
        public bool SavesRecents { get; set; }
        public List<string> OwnedDlcs { get; set; }
        public double[] Position { get; set; }
        public List<string> Favorite { get; set; }
        public int AllTrackVersion { get; set; }
        #endregion

        #region Methods
        public void Import()
        {
            try
            {
                using (var reader = new StreamReader(CONFIG))
                {
                    string json = reader.ReadToEnd();
                    Setting setting = JsonSerializer.Deserialize<Setting>(json);

                    RecentsCount = setting.RecentsCount;
                    Mode = setting.Mode;
                    Aider = setting.Aider;
                    Level = setting.Level;
                    InputDelay = setting.InputDelay;
                    SavesRecents = setting.SavesRecents;
                    OwnedDlcs = setting.OwnedDlcs;
                    Position = setting.Position;
                    Favorite = setting.Favorite;
                    AllTrackVersion = setting.AllTrackVersion;
                }
            }
            catch (FileNotFoundException)
            {
                RecentsCount = 5;
                Mode = Mode.Freestyle;
                Aider = Aider.Off;
                Level = Level.Off;
                InputDelay = 30;
                SavesRecents = false;
                OwnedDlcs = new List<string>();
                Position = new double[2];
                Favorite = new List<string>();
                AllTrackVersion = 0;
            }
        }
        public void Export()
        {
            var options = new JsonSerializerOptions()
            { 
                WriteIndented = true,
                IgnoreReadOnlyProperties = false
            };
            string jsonString = JsonSerializer.Serialize(this, options);

            using (var writer = new StreamWriter(CONFIG))
            {
                writer.Write(jsonString);
            }
        }
        #endregion
    }
}
