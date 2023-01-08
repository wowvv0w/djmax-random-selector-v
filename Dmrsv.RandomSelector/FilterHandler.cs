using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.RandomSelector
{
    public delegate IEnumerable<Music> FilteringMethod<TFilter>(IEnumerable<Track> trackList, TFilter filter) where TFilter : IFilter;

    public class FilterHandler<TFilter> : IFilterHandler<TFilter> where TFilter : IFilter
    {
        private FilteringMethod<TFilter> _filteringMethod;

        public FilterHandler()
        {
            _filteringMethod = (t, f) => t.SelectMany((t) => t.GetMusicList());
        }

        public IEnumerable<Music> Filter(IEnumerable<Track> trackList, TFilter filter)
        {
            return _filteringMethod.Invoke(trackList, filter);
        }

        public void SetFilteringMethod(FilteringMethod<TFilter> method) => _filteringMethod = method;
    }
}
