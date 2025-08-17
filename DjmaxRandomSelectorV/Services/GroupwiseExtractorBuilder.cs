using DjmaxRandomSelectorV.Enums;
using DjmaxRandomSelectorV.Extractors;

namespace DjmaxRandomSelectorV.Services
{
    public class GroupwiseExtractorBuilder : IGroupwiseExtractorBuilder
    {
        public MusicForm StyleType { get; set; }
        public LevelPreference LevelPreference { get; set; }

        public IGroupwiseExtractor Build()
        {
            return (StyleType, LevelPreference) switch
            {
                (MusicForm.Free, _) => new UniquePatternExtractor(),
                (MusicForm.Default, LevelPreference.Lowest) => new EasiestPatternExtractor(),
                (MusicForm.Default, LevelPreference.Highest) => new HardestPatternExtractor(),
                _ => null
            };
        }
    }
}
