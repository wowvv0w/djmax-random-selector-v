using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Playlist
    {
        #region Fields
        private List<Music> _musicList;
        private List<Music> _recents;
        private bool _isUpdated;
        #endregion

        private const string CurrentPlaylistPath = "Data/CurrentPlaylist.json";

        #region Properties
        public List<Music> MusicList
        {
            get { return _musicList; }
            set { _musicList = value; }
        }
        public List<Music> Recents
        {
            get { return _recents; }
            set { _recents = value; }
        }
        [JsonIgnore]
        public bool IsUpdated
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }
        #endregion

        public Playlist()
        {
            _isUpdated = true;
        }

        #region Methods
        public void UpdateRecents(int musicCount, int maxCount)
        {
            int recentsCount = _recents.Count;
            if (recentsCount > maxCount)
            {
                _recents.RemoveRange(0, recentsCount - maxCount);
            }
            else if (recentsCount >= musicCount)
            {
                try
                {
                    _recents.RemoveRange(0, recentsCount - musicCount + 1);
                }
                catch (ArgumentException)
                {
                }
            }
        }
        public void Import(string presetPath = null)
        {
            bool isNull = string.IsNullOrEmpty(presetPath);
            var path = isNull ? CurrentPlaylistPath : presetPath;
            try
            {
                using (var reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    Playlist playlist = JsonSerializer.Deserialize<Playlist>(json);

                    MusicList = playlist.MusicList;
                    Recents = playlist.Recents;
                }
            }
            catch (FileNotFoundException e)
            {
                if (isNull)
                {
                    MusicList = new List<Music>();
                    Recents = new List<Music>();
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
            var path = string.IsNullOrEmpty(presetPath) ? CurrentPlaylistPath : presetPath;

            using (var writer = new StreamWriter(path))
            {
                writer.Write(jsonString);
            }
        }
        #endregion
    }
}
