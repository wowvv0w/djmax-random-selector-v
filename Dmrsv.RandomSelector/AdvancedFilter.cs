using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public class AdvancedFilter : FilterBase
    {
        private ObservableCollection<Pattern> _patternList;

        public ObservableCollection<Pattern> PatternList
        {
            get => _patternList;
            set
            {
                _patternList = value;
                _patternList.CollectionChanged += (s, e) => IsUpdated = true;
            }
        }

        public AdvancedFilter()
        {
            _patternList = new ObservableCollection<Pattern>();
            _patternList.CollectionChanged += (s, e) => IsUpdated = true;
        }

        public override IEnumerable<Pattern> Filter(IEnumerable<Track> trackList)
        {
            IsUpdated = false;
            return _patternList;
        }
    }
}
