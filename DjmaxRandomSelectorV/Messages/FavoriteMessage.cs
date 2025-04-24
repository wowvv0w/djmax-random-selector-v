using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Messages
{
    public record FavoriteMessage(List<int> Favorite, List<int> Blacklist);
}
