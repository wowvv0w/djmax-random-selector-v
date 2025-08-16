namespace Dmrsv.RandomSelector
{
    public record Pattern
    {
        public PatternId Id { get; init; } = default;
        public MusicInfo Info { get; init; } = new();
        public int Level { get; init; } = default;

        public int TrackId => Id.TrackId;
        public ButtonTunes Button => Id.Button;
        public Difficulty Difficulty => Id.Difficulty;
        public string Style => Button.AsString() + Difficulty.AsString();
    }
}
