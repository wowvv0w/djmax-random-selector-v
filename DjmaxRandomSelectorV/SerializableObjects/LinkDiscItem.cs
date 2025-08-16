using System.Collections.Generic;
using System.Linq;

namespace DjmaxRandomSelectorV.SerializableObjects
{
    public record LinkDiscItem(int Id, string[][] RequiredDlc)
    {
        public bool IsRequired(IEnumerable<string> ownedDlcs)
        {
            return RequiredDlc.Any(required => required.All(dlc => ownedDlcs.Contains(dlc)));
        }
    }
}
