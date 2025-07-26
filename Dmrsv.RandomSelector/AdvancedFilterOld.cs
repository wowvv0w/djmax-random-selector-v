using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public class AdvancedFilterOld : FilterBase
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

        public AdvancedFilterOld()
        {
            _patternList = new ObservableCollection<Pattern>();
            _patternList.CollectionChanged += (s, e) => IsUpdated = true;
        }

        public override IEnumerable<Pattern> Filter(IEnumerable<Track> trackList)
        {
            var result = from pattern in _patternList
                         where trackList.Any(track => track.Id == pattern.Info.Id)
                         select pattern;
            IsUpdated = false;
            return result;
        }
    }
}
