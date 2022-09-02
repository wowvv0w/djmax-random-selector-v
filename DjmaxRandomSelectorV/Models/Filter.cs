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
        private List<string> _buttonTunes;
        private List<string> _difficulties;
        private List<string> _categories;
        private int[] _levels;
        private int[] _scLevels;
        private List<string> _recents;
        private bool _includesFavorite;
        private bool _isUpdated;
        #endregion

        #region Properties
        public List<string> ButtonTunes
        {
            get { return _buttonTunes; }
            set { _buttonTunes = value; }
        }
        public List<string> Difficulties
        {
            get { return _difficulties; }
            set { _difficulties = value; }
        }
        public List<string> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }
        public int[] Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
        public int[] ScLevels
        {
            get { return _scLevels; }
            set { _scLevels = value; }
        }
        public List<string> Recents
        {
            get { return _recents; }
            set { _recents = value; }
        }
        public bool IncludesFavorite
        {
            get { return _includesFavorite; }
            set { _includesFavorite = value; }
        }
        [JsonIgnore]
        public bool IsUpdated
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }
        #endregion

        public Filter()
        {
            _isUpdated = true;
        }
    }
}
