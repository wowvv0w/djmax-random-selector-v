using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DjmaxRandomSelectorV.Models
{
    public class Setting : IAddonObservable
    {
        #region Fields
        private Mode mode;
        private Aider aider;
        private Level level;
        private readonly List<IAddonObserver> observers;
        #endregion

        #region Constants
        private const string CONFIG = "Data/Config.json";
        #endregion

        #region Properties
        public int RecentsCount { get; set; }
        public Mode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                Notify();
            }
        }
        public Aider Aider
        {
            get { return aider; }
            set
            {
                aider = value;
                Notify();
            }
        }
        public Level Level
        {
            get { return level; }
            set
            {
                level = value;
                Notify();
            }
        }
        public bool IsPlaylist { get; set; }
        public int InputDelay { get; set; }
        public bool SavesRecents { get; set; }
        public List<string> OwnedDlcs { get; set; }
        public double[] Position { get; set; }
        public List<string> Favorite { get; set; }
        public int AllTrackVersion { get; set; }
        #endregion

        public Setting()
        {
            observers = new List<IAddonObserver>();
        }

        #region IAddonObservable Methods
        public void Subscribe(IAddonObserver observer) => observers.Add(observer);
        public void Notify()
        {
            foreach (var observer in observers)
                observer.Update(this);
        }
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
                    IsPlaylist = setting.IsPlaylist;
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
                IsPlaylist = false;
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
