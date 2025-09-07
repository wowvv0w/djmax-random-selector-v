using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Conditions;

namespace DjmaxRandomSelectorV.Models
{
    public class ConditionNodeItem : PropertyChangedBase
    {
		public Type ConditionType { get; }
		public ObservableCollection<object> ConditionArgs { get; }

		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				_isEnabled = value;
				NotifyOfPropertyChange();
			}
		}

		private bool _isNegation = false;
		public bool IsNegation
		{
			get => _isNegation;
			set
			{
				_isNegation = value;
				NotifyOfPropertyChange();
			}
		}

		public ObservableCollection<ConditionNodeItem> Children { get; }

		public ConditionNodeItem(Type type, params object[] args)
		{
			if (type.IsAssignableTo(typeof(ICompoundCondition)))
			{
				var conditions = Children
					.Select(node => node.ToCondition())
					.Where(cond => cond is not null);
				ConditionArgs = new ObservableCollection<object>() { conditions };
			}
			else
			{
				ConditionArgs = new ObservableCollection<object>(args);
			}
		}

		public ICondition ToCondition()
		{
			if (!IsEnabled)
			{
				return null;
			}

			ICondition result = (ICondition)Activator.CreateInstance(ConditionType, ConditionArgs);
			return IsNegation ? Condition.Not(result) : result;
		}
	}
}
