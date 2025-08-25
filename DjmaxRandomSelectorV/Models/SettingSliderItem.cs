using System;
using Caliburn.Micro;

namespace DjmaxRandomSelectorV.Models
{
    public class SettingSliderItem : PropertyChangedBase
    {
        private readonly Func<int> _valueGetter;
        private readonly Action<int> _onValueChanged;
        private readonly Func<int, string> _valueTextFormatter;

        public string Name { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public int Tick { get; }

        public int Value
        {
            get => _valueGetter.Invoke();
            set
            {
                _onValueChanged.Invoke(value);
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ValueText));
            }
        }

        public string ValueText => _valueTextFormatter?.Invoke(Value) ?? Value.ToString();

        public SettingSliderItem(
            string name,
            int minValue,
            int maxValue,
            int tick,
            Func<int> valueGetter,
            Action<int> onValueChanged,
            Func<int, string> valueTextFormatter = null)
        {
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            Tick = tick;
            _valueGetter = valueGetter;
            _onValueChanged = onValueChanged;
            _valueTextFormatter = valueTextFormatter;
        }
    }
}
