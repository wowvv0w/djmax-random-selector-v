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
        }

        #region Import

        internal T Import<T>(string path) where T : new()
        {
            try
            {
                using var reader = new StreamReader(path);

                string json = reader.ReadToEnd();
                T instance = JsonSerializer.Deserialize<T>(json);

                return instance;
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

        private ConditionalFilter _conditionalFilter;
        private SelectiveFilter _selectiveFilter;
        private FilterOption _filterOption;
        private SelectorOption _selectorOption;
        private ExtraFilter _extraFilter;
        private AppOption _appOption;
        #endregion

        internal ConditionalFilter ConditionalFilter
        {
            get { return _conditionalFilter ??= Import<ConditionalFilter>(GetJsonPath(nameof(ConditionalFilter))); }
            set
            {
                _conditionalFilter = value;
                Export(_conditionalFilter, GetJsonPath(nameof(ConditionalFilter)));
            }
        }

        internal SelectiveFilter SelectiveFilter
        {
            get { return _selectiveFilter ??= Import<SelectiveFilter>(GetJsonPath(nameof(SelectiveFilter))); }
            set
            {
                _selectiveFilter = value;
                Export(_selectiveFilter, GetJsonPath(nameof(SelectiveFilter)));
            }
        }

        internal FilterOption FilterOption
        {
            get { return _filterOption ??= Import<FilterOption>(GetJsonPath(nameof(FilterOption))); }
            set
            {
                _filterOption = value;
                Export(_filterOption, GetJsonPath(nameof(FilterOption)));
            }
        }

        internal SelectorOption SelectorOption
        {
            get { return _selectorOption ??= Import<SelectorOption>(GetJsonPath(nameof(SelectorOption))); }
            set
            {
                _selectorOption = value;
                Export(_selectorOption, GetJsonPath(nameof(SelectorOption)));
            }
        }

        internal ExtraFilter ExtraFilter
        {
            get { return _extraFilter ??= Import<ExtraFilter>(GetJsonPath(nameof(ExtraFilter))); }
            set
            {
                _extraFilter = value;
                Export(_extraFilter, GetJsonPath(nameof(ExtraFilter)));
            }
        }

        internal AppOption AppOption
        {
            get { return _appOption ??= Import<AppOption>(GetJsonPath(nameof(AppOption))); }
            set
            {
                _appOption = value;
                Export(_appOption, GetJsonPath(nameof(AppOption)));
            }
        }
    }
}
