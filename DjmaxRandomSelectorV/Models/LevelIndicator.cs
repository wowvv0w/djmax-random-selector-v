using Caliburn.Micro;

namespace DjmaxRandomSelectorV.Models
{
    public class LevelIndicator : PropertyChangedBase
    {
        private readonly int _number;
        private readonly int[] _source;

        public int Number { get => _number; }
        public bool Value
        {
            get => _source[0] <= _number && _number <= _source[1];
        }
        public LevelIndicator(int number, int[] source)
        {
            _number = number;
            _source = source;
        }
    }
}
