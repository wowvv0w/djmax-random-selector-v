using System;
using Caliburn.Micro;

namespace DjmaxRandomSelectorV.Models
{
    public class SettingToggleItem : PropertyChangedBase
    {
        private readonly Func<bool> _toggleStateGetter;
        private readonly Action<bool> _onToggleChanged;

        public string Name { get; }

        public bool IsOn
        {
            get => _toggleStateGetter.Invoke();
            set
            {
                _onToggleChanged.Invoke(value);
                NotifyOfPropertyChange();
            }
        }

        public SettingToggleItem(string name, Func<bool> toggleStateGetter, Action<bool> onToggleChanged)
        {
            Name = name;
            _toggleStateGetter = toggleStateGetter;
            _onToggleChanged = onToggleChanged;
        }
    }
}
