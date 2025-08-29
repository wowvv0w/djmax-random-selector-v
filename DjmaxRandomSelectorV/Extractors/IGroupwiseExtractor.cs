using System.Collections.Generic;
using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Extractors
{
    public interface IGroupwiseExtractor
    {
        IEnumerable<Pattern> Extract(IEnumerable<Pattern> patterns);
    }
}
