using Caliburn.Micro;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public class ListUpdater : PropertyChangedBase
    {
        private readonly string _name;
        private readonly string _value;
        private readonly List<string> _target;

        public string Name { get => _name; }
        public bool IsValueContained
        {
            get => _target.Contains(_value);
            set
            {
                if (value)
                {
                    _target.Add(_value);
                }
                else
                {
                    _target.Remove(_value);
                }
                NotifyOfPropertyChange();
            }
        }

        public ListUpdater(string name, string value, List<string> target)
        {
            _name = name;
            _value = value;
            _target = target;
        }
    }
}
