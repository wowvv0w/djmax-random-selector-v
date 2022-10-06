using Dmrsv.Data.Context.Core;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.DataTypes;

namespace Dmrsv.Data.Controller
{
    public class FilterApi : DmrsvDataContext
    {
        public QueryFilter GetQueryFilter()
        {
            var filter = Data.QueryFilter;
            return new QueryFilter()
            {
                ButtonTunes = new List<string>(filter.ButtonTunes),
                Difficulties = new List<string>(filter.Difficulties),
                Categories = new List<string>(filter.Categories),
                Levels = (int[])filter.Levels.Clone(),
                ScLevels = (int[])filter.Levels.Clone(),
                IncludesFavorite = filter.IncludesFavorite,
            };
        }

        public void SetQueryFilter(QueryFilter filter)
        {
            Data.QueryFilter = new QueryFilter()
            {
                ButtonTunes = new List<string>(filter.ButtonTunes),
                Difficulties = new List<string>(filter.Difficulties),
                Categories = new List<string>(filter.Categories),
                Levels = (int[])filter.Levels.Clone(),
                ScLevels = (int[])filter.Levels.Clone(),
                IncludesFavorite = filter.IncludesFavorite,
            };
        }

        public QueryFilter GetPreset(string path)
        {
            return Data.Import<QueryFilter>(path);
        }

        public void SetPreset(QueryFilter filter, string path)
        {
            Data.Export(filter, path);
        }

        public PlaylistFilter GetPlaylistFilter()
        {
            var filter = Data.PlaylistFilter;
            return new PlaylistFilter()
            {
                Playlist = new List<PlaylistItem>(filter.Playlist),
            };
        }

        public void SetPlaylistFilter(PlaylistFilter filter)
        {
            Data.PlaylistFilter = new PlaylistFilter()
            {
                Playlist = new List<PlaylistItem>(filter.Playlist),
            };
        }

        public PlaylistFilter GetPlaylist(string path)
        {
            return Data.Import<PlaylistFilter>(path);
        }

        public void SetPlaylist(PlaylistFilter filter, string path)
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

        public void SaveQueryFilter()
        {
            Data.Export(Data.QueryFilter, "Data/CurrentFilter.json");
        }

        public void SavePlaylistFilter()
        {
            Data.Export(Data.PlaylistFilter, "Data/CurrentFilter.json");
        }
    }
}
