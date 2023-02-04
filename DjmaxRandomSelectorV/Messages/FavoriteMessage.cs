using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Messages
{
    public record FavoriteMessage(List<string> Favorite, List<string> Blacklist);
}
