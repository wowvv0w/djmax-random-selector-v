using System;
using Caliburn.Micro;

namespace DjmaxRandomSelectorV.Models
{
    public class SettingSpinBoxItem : PropertyChangedBase
    {
        // TODO: SettingItemBase
        private readonly Func<int> _valueSource;
        private readonly Action<int> _onValueChanged;
        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly bool _isCircular;
        private readonly Func<int, string> _valueTextFormatter;

        private readonly int _count;

        public string Name { get; }

        public int Value
        {
            get => _valueSource.Invoke();
            set
            {
                _onValueChanged.Invoke(value);
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ValueText));
            }
        }

        public string ValueText => _valueTextFormatter?.Invoke(Value) ?? Value.ToString();

        public SettingSpinBoxItem(
            string name,
            Func<int> valueSource,
            Action<int> onValueChanged,
            int minValue,
            int maxValue,
            bool isCircular = false,
            Func<int, string> valueTextFormatter = null)
        {
            Name = name;
            _valueSource = valueSource;
            _onValueChanged = onValueChanged;
            _minValue = minValue;
            _maxValue = maxValue;
            _isCircular = isCircular;
            _valueTextFormatter = valueTextFormatter;

            _count = _maxValue - _minValue + 1;
        }

        public void PrevValue()
        {
            SetValue(Value - 1);
        }

        public void NextValue()
        {
            SetValue(Value + 1);
        }

        private void SetValue(int newValue)
        {
            if (_minValue <= newValue && newValue <= _maxValue)
            {
                Value = newValue;
            }
            else if (_isCircular)
            {
                Value = (newValue + _count) % _count;
            }
        }
    }
}
