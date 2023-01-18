using Caliburn.Micro;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Models
{
    public class CategoryUpdater : PropertyChangedBase
    {
        private readonly string _value;
        private readonly List<string> _target;

        public string Name { get; }
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

        public CategoryUpdater(Category category, List<string> target)
        {
            Name = category.Name;
            _value = category.Id;
            _target = target;
        }
    }
}
