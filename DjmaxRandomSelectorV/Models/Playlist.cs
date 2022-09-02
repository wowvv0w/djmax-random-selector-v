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
    }
}
