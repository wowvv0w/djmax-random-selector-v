using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Conditions
{
    public interface ICompoundCondition : ICondition
    {
        IEnumerable<ICondition> Conditions { get; }
    }
}
