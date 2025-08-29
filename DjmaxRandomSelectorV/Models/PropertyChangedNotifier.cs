using System;
using Caliburn.Micro;

namespace DjmaxRandomSelectorV.Models
{
    public class PropertyChangedNotifier<T> : PropertyChangedBase
    {
        private readonly Func<T> _valueGetter;
        private readonly Action<T> _onValueChanged;

        public string Name { get; }
        public T Value
        {
            get => _valueGetter.Invoke();
            set
            {
                _onValueChanged.Invoke(value);
                NotifyOfPropertyChange();
            }
        }

        public PropertyChangedNotifier(string name, Func<T> valueGetter, Action<T> onValueChanged)
        {
            Name = name;
            _valueGetter = valueGetter;
            _onValueChanged = onValueChanged;
        }
    }
}
