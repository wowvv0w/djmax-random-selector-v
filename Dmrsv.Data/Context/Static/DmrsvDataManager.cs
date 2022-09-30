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
            _cdtFilter = Import<ConditionalFilter>(GetJsonPath("CurrentFilter"));
            _slcFilter = Import<SelectiveFilter>(GetJsonPath("CurrentPlaylist"));
            _config = Import<Config>(GetJsonPath("Config"));
            _filterOption = new FilterOption()
            {
                Except = _config.RecentsCount,
                Mode = _config.Mode,
                Aider = _config.Aider,
                Level = _config.Level,
            };
            _selectorOption = new SelectorOption()
            {
                FilterType = _config.FilterType,
                InputInterval = _config.InputDelay,
                OwnedDlcs = _config.OwnedDlcs,
                SavesExclusion = _config.SavesRecents,
            };
            _extraFilter = new ExtraFilter()
            {
                Exclusions = _config.Exclusions,
                Favorites = _config.Favorite,
            };
            _appOption ??= new AppOption()
            {
                AllTrackVersion = _config.AllTrackVersion,
                Position = _config.Position,
            };
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

        private ConditionalFilter _cdtFilter;
        private SelectiveFilter _slcFilter;
        private Config _config;
        private FilterOption _filterOption;
        private SelectorOption _selectorOption;
        private ExtraFilter _extraFilter;
        private AppOption _appOption;
        #endregion

        internal ConditionalFilter ConditionalFilter
        {
            get { return _cdtFilter; }
            set
            {
                _cdtFilter = value;
                Export(_cdtFilter, GetJsonPath(nameof(ConditionalFilter)));
            }
        }

        internal SelectiveFilter SelectiveFilter
        {
            get { return _slcFilter; }
            set
            {
                _slcFilter = value;
                Export(_slcFilter, GetJsonPath(nameof(SelectiveFilter)));
            }
        }

        internal FilterOption FilterOption
        {
            get { return _filterOption; }
            set
            {
                _filterOption.Except = value.Except;
                _filterOption.Mode = value.Mode;
                _filterOption.Aider = value.Aider;
                _filterOption.Level = value.Level;
            }
        }

        internal SelectorOption SelectorOption
        {
            get { return _selectorOption; }
            set
            {
                _selectorOption.FilterType = value.FilterType;
                _selectorOption.InputInterval = value.InputInterval;
                _selectorOption.OwnedDlcs = value.OwnedDlcs;
                _selectorOption.SavesExclusion = value.SavesExclusion;
            }
        }

        internal ExtraFilter ExtraFilter
        {
            get { return _extraFilter; }
            set
            {
                _extraFilter.Exclusions = value.Exclusions;
                _extraFilter.Favorites = value.Favorites;
            }
        }

        internal AppOption AppOption
        {
            get { return _appOption; }
            set
            {
                _appOption.AllTrackVersion = value.AllTrackVersion;
                _appOption.Position = value.Position;
            }
        }
    }
}
