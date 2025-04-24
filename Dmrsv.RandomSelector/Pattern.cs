using Dmrsv.RandomSelector.Enums;

namespace Dmrsv.RandomSelector
{
    public record Pattern
    {
        public MusicInfo Info { get; init; } = new();
        public ButtonTunes Button { get; init; } = default;
        public Difficulty Difficulty { get; init; } = default;
        public int Level { get; init; } = default;

        public int TrackId => Info.Id;
        public int PatternId => TrackId * 100 + (int)Button * 10 + (int)Difficulty;
        public string Style => Button.AsString() + Difficulty.AsString();
    }
}
