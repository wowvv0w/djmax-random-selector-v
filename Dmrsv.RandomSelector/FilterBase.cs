using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public abstract class FilterBase : IFilter
    {
        private OutputMethodCallback? _outputMethod;

        [JsonIgnore]
        public bool IsUpdated { get; protected set; }
        [JsonIgnore]
        public OutputMethodCallback? OutputMethod
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

        public abstract List<Music> Filter(IEnumerable<OldTrack> trackList);
    }
}
