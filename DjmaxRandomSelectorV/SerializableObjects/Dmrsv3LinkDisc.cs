using System.Collections.Generic;
using System.Linq;

namespace DjmaxRandomSelectorV.SerializableObjects
{
    public class Dmrsv3LinkDisc
    {
        public int Id { get; set; }
        public string[][] RequiredDlc { get; set; }

        public bool IsRequired(IEnumerable<string> ownedDlcs)
        {
            return RequiredDlc.Any(required => required.All(dlc => ownedDlcs.Contains(dlc)));
        }
    }
}
