using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.RandomSelector
{
    public class QueryFilterHandlerBuilder : IFilterHandlerBuilder
    {
        private FilterHandler<QueryFilter>? _instance;

        public QueryFilterHandlerBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _instance = new FilterHandler<QueryFilter>();
        }

        public void Build(FilterHandlerOption option)
        {
            var trackPredicate = (Track t, QueryFilter filter) =>
            {
                var categories = filter.Categories;
                var favorites = categories.Contains("FAVORITE") ? filter.Favorites : new List<string>();
                var blacklist = filter.Blacklist;
                return (categories.Contains(t.Category) || favorites.Contains(t.Title))
                        && !blacklist.Contains(t.Title);
            };

            var filterTrack = (IEnumerable<Track> tl, QueryFilter f) 
                => tl.Where((t) => trackPredicate.Invoke(t, f));

            var musicPredicate = (Music m, QueryFilter f) =>
            {
                string bt = m.ButtonTunes, dif = m.Difficulty;
                if (!f.ButtonTunes.Contains(bt) || !f.Difficulties.Contains(dif))
                {
                    return false;
                }

                int[] levels = m.Difficulty == "SC" ? f.ScLevels : f.Levels;
                int min = levels[0], max = levels[1];
                return min <= m.Level && m.Level <= max;
            };

            FilteringMethod<QueryFilter> filteringMethod;

            if (option.Mode == MusicForm.Free)
            {
                filteringMethod = (trackList, filter) =>
                {
                    var result = from t in filterTrack.Invoke(trackList, filter)
                                 let ml = t.GetMusicList()
                                 where ml.Any(m => musicPredicate.Invoke(m, filter))
                                 select new Music()
                                 {
                                     Title = t.Title,
                                     ButtonTunes = string.Empty,
                                     Difficulty = string.Empty,
                                     Level = -1
                                 };
                    return result;
                };

                _instance!.SetFilteringMethod(filteringMethod);
                return;
            }

            Func<IEnumerable<Music>, IEnumerable<Music>> outputMethod = option.Level switch
            {
                LevelPreference.None => (ml) => ml,
                LevelPreference.Lowest => (ml) =>
                {
                    ml = from m in ml
                         group m by new { title = m.Title, buttonTunes = m.ButtonTunes } into g
                         select g.First();
                    return ml;
                },
                LevelPreference.Highest => (ml) =>
                {
                    ml = from m in ml
                         group m by new { title = m.Title, buttonTunes = m.ButtonTunes } into g
                         select g.Last();
                    return ml;
                },
                _ => throw new NotImplementedException()
            };

            filteringMethod = (trackList, filter) =>
            {
                var result = from t in filterTrack.Invoke(trackList, filter)
                             from m in t.GetMusicList()
                             where musicPredicate.Invoke(m, filter)
                             select m;
                return outputMethod.Invoke(result);
            };

            _instance!.SetFilteringMethod(filteringMethod);
            return;
        }

        public FilterHandler<QueryFilter> Get()
        {
            FilterHandler<QueryFilter> instance = _instance!;
            Reset();
            return instance;
        }
    }
}
