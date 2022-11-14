using Dmrsv.Data.Context.Schema;
using System.Text.Json;

namespace Dmrsv.Data.Context.Static
{
    public class DmrsvDataManager
    {
        internal static DmrsvDataManager Instance;

        static DmrsvDataManager()
        {
            Instance = new DmrsvDataManager();
        }

        private DmrsvDataManager()
        {
            _queryFilter = Import<QueryFilter>(GetJsonPath("CurrentFilter"));
            _playlistFilter = Import<PlaylistFilter>(GetJsonPath("CurrentPlaylist"));
            _config = Import<Config>(GetJsonPath("Config"));
        }

        #region Import

        internal T Import<T>(string path) where T : new()
        {
            try
            {
                using var reader = new StreamReader(path);

                string json = reader.ReadToEnd();
                T? instance = JsonSerializer.Deserialize<T>(json);

                return instance ?? new T();
            }
            catch (FileNotFoundException)
            {
                return new T();
            }
        }
        #endregion

        #region Export

        internal void Export<T>(T instance, string path)
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
        #endregion

        #region GetJsonPath

        private string GetJsonPath(string fileName)
        {
            return $"Data/{fileName}.json";
        }
        #endregion

        #region Fields

        private QueryFilter _queryFilter;
        private PlaylistFilter _playlistFilter;
        private Config _config;
        #endregion

        internal QueryFilter QueryFilter
        {
            get { return _queryFilter; }
            set { _queryFilter = value; }
        }

        internal PlaylistFilter PlaylistFilter
        {
            get { return _playlistFilter; }
            set { _playlistFilter = value; }
        }

        internal Config Config
        {
            get { return _config; }
        }

        internal FilterOption FilterOption
        {
            get
            {
                return new FilterOption()
                {
                    Except = _config.RecentsCount,
                    Mode = _config.Mode,
                    Aider = _config.Aider,
                    Level = _config.Level,
                };
            }
            set
            {
                _config.RecentsCount = value.Except;
                _config.Mode = value.Mode;
                _config.Aider = value.Aider;
                _config.Level = value.Level;
            }
        }

        internal SelectorOption SelectorOption
        {
            get
            {
                return new SelectorOption()
                {
                    FilterType = _config.FilterType,
                    InputInterval = _config.InputDelay,
                    OwnedDlcs = _config.OwnedDlcs,
                    SavesExclusion = _config.SavesRecents,
                };
            }
            set
            {
                _config.FilterType = value.FilterType;
                _config.InputDelay = value.InputInterval;
                _config.OwnedDlcs = value.OwnedDlcs;
                _config.SavesRecents = value.SavesExclusion;
            }
        }

        internal ExtraFilter ExtraFilter
        {
            get
            {
                return new ExtraFilter()
                {
                    Exclusions = _config.Exclusions,
                    Favorites = _config.Favorite,
                    Blacklist = _config.Blacklist,
                };
            }
            set
            {
                _config.Exclusions = value.Exclusions;
                _config.Favorite = value.Favorites;
                _config.Blacklist = value.Blacklist;
            }
        }

        internal AppOption AppOption
        {
            get
            {
                return new AppOption()
                {
                    AllTrackVersion = _config.AllTrackVersion,
                    Position = _config.Position,
                };
            }
            set
            {
                _config.AllTrackVersion = value.AllTrackVersion;
                _config.Position = value.Position;
            }
        }
    }
}
