using Caliburn.Micro;

namespace DjmaxRandomSelectorV.Models
{
    public class VArchivePatternItem : PropertyChangedBase
    {
        private bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                NotifyOfPropertyChange();
            }
        }
        public int Id { get; init; }
        public string Style { get; init; }
        public string Title { get; init; }
        public double Floor { get; init; }
        public double? Score { get; init; }
        public bool? IsMaxCombo { get; init; }
    }
}
