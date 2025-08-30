using System;

namespace DjmaxRandomSelectorV.Conditions
{
    public sealed class ConditionInfo
    {
        private Type _type;
        public Type Type
        {
            get => _type;
            set
            {
                Condition.ThrowIfNotCondition(value);
                _type = value;
            }
        }
        public object[] Args { get; set; }

        public bool IsNegation => Type == typeof(ComplementCondition);
        public bool IsMergedCondition => Type.IsAssignableTo(typeof(IMergedCondition));

        public ICondition Create()
        {
            return Condition.CreateInstance(this);
        }
    }
}
