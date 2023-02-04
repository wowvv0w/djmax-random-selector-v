using Dmrsv.RandomSelector;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Messages
{
    public record VArchiveMessage(IEnumerable<Music> Items, string Command);
}
