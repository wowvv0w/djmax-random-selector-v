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


        // Obsolete

        public static int CreateId(int trackId, string style)
        {
            int buttonTunes = style[..2] switch
            {
                "4B" => 4,
                "5B" => 5,
                "6B" => 6,
                "8B" => 8,
                _ => throw new NotSupportedException("Invalid ButtonTunes")
            };
            int difficulty = style[2..4] switch
            {
                "NM" => 1,
                "HD" => 2,
                "MX" => 3,
                "SC" => 4,
                _ => throw new NotSupportedException("Invalid Difficulty")
            };
            return trackId * 100 + buttonTunes * 10 + difficulty;
        }
    }
}
