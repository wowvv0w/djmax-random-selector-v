using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public abstract class FilterBase : IFilter
    {
        private Func<IEnumerable<Music>, IEnumerable<Music>>? _outputMethod;

        [JsonIgnore]
        public bool IsUpdated { get; protected set; }
        [JsonIgnore]
        public Func<IEnumerable<Music>, IEnumerable<Music>>? OutputMethod
        {
            get { return _outputMethod; }
            set
            {
                _outputMethod = value;
                IsUpdated = true;
            }
        }

        public FilterBase()
        {
            IsUpdated = true;
            OutputMethod = null;
        }

        public abstract List<Music> Filter(IEnumerable<Track> trackList);
    }
}
