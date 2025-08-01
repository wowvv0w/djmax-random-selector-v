using System.Collections.Generic;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Extractors
{
    public interface IGroupwiseExtractor
    {
        IEnumerable<Pattern> Extract(IEnumerable<Pattern> patterns);
    }
}
