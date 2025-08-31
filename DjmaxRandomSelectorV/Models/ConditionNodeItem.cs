using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Conditions;

namespace DjmaxRandomSelectorV.Models
{
    public class ConditionNodeItem : PropertyChangedBase
    {
		private readonly ConditionInfo _conditionInfo;

		public Type ConditionType => _conditionInfo.Type;
		public object[] ConditionArgs => _conditionInfo.Args;

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
			_conditionInfo = new ConditionInfo() { Type = type };

			if (_conditionInfo.IsMergedCondition)
			{
				var conditions = Children
					.Select(node => node.ToCondition())
					.Where(cond => cond is not null);
				_conditionInfo.Args = new[] { conditions };
			}
			else
			{
				_conditionInfo.Args = args;
			}
		}

		public ICondition ToCondition()
		{
			if (!IsEnabled)
			{
				return null;
			}

			ICondition result = _conditionInfo.Create();
			if (IsNegation)
			{
				result = Condition.Not(result);
			}
			return result;
		}
	}
}
