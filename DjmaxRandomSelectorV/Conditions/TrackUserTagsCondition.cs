using DjmaxRandomSelectorV.RandomSelector;

namespace DjmaxRandomSelectorV.Conditions
{
    public record TrackUserTagsCondition(TrackUserTags UserTags) : ICondition
    {
        public bool IsSatisfiedBy(Pattern pattern)
        {
            return pattern.TrackUserTags.HasFlag(UserTags);
        }
    }
}
