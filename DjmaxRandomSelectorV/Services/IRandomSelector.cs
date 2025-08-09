using System.Collections.Generic;
using DjmaxRandomSelectorV.Conditions;
using DjmaxRandomSelectorV.Extractors;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public interface IRandomSelector
    {
        void SetCandidates(IEnumerable<Track> playable, ICondition condition, IGroupwiseExtractor extractor);
        Pattern Select();
        Pattern Reselect();
    }
}
