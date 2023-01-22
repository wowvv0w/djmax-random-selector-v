using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public abstract class FilterBase : IFilter
    {
        [JsonIgnore]
        public bool IsUpdated { get; protected set; }
        [JsonIgnore]
        public Func<IEnumerable<Music>, IEnumerable<Music>>? OutputMethod { get; set; }

        public FilterBase()
        {
            IsUpdated = true;
            OutputMethod = null;
        }

        public abstract IEnumerable<Music> Filter(IEnumerable<Track> trackList);
    }
}
