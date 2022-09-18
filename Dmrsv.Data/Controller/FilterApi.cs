using Dmrsv.Data.Context.Core;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.DataTypes;

namespace Dmrsv.Data.Controller
{
    public class FilterApi : DmrsvDataContext
    {
        public ConditionalFilter GetConditionalFilter()
        {
            var filter = Data.ConditionalFilter;
            return new ConditionalFilter()
            {
                ButtonTunes = new List<string>(filter.ButtonTunes),
                Difficulties = new List<string>(filter.Difficulties),
                Categories = new List<string>(filter.Categories),
                Levels = (int[])filter.Levels.Clone(),
                ScLevels = (int[])filter.Levels.Clone(),
                IncludesFavorite = filter.IncludesFavorite,
            };
        }

        public void SetConditionalFilter(ConditionalFilter filter)
        {
            Data.ConditionalFilter = new ConditionalFilter()
            {
                ButtonTunes = new List<string>(filter.ButtonTunes),
                Difficulties = new List<string>(filter.Difficulties),
                Categories = new List<string>(filter.Categories),
                Levels = (int[])filter.Levels.Clone(),
                ScLevels = (int[])filter.Levels.Clone(),
                IncludesFavorite = filter.IncludesFavorite,
            };
        }

        public ConditionalFilter GetPreset(string path)
        {
            return Data.Import<ConditionalFilter>(path);
        }

        public void SetPreset(ConditionalFilter filter, string path)
        {
            Data.Export(filter, path);
        }

        public SelectiveFilter GetSelectiveFilter()
        {
            var filter = Data.SelectiveFilter;
            return new SelectiveFilter()
            {
                Playlist = new List<PlaylistItem>(filter.Playlist),
            };
        }

        public void SetSelectiveFilter(SelectiveFilter filter)
        {
            Data.SelectiveFilter = new SelectiveFilter()
            {
                Playlist = new List<PlaylistItem>(filter.Playlist),
            };
        }

        public SelectiveFilter GetPlaylist(string path)
        {
            return Data.Import<SelectiveFilter>(path);
        }

        public void SetPlaylist(SelectiveFilter filter, string path)
        {
            Data.Export(filter, path);
        }

        public ExtraFilter GetExtraFilter()
        {
            var filter = Data.ExtraFilter;
            return new ExtraFilter()
            {
                Exclusions = new List<string>(filter.Exclusions),
                Favorites = new List<string>(filter.Favorites),
            };
        }

        public void SetExtraFilter(ExtraFilter filter)
        {
            Data.ExtraFilter = new ExtraFilter()
            {
                Exclusions = new List<string>(filter.Exclusions),
                Favorites = new List<string>(filter.Favorites),
            };
        }
    }
}
