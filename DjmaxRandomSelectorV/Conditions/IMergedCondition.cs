using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Conditions
{
    public interface IMergedCondition : ICondition
    {
        IEnumerable<ICondition> Conditions { get; }
    }
}
