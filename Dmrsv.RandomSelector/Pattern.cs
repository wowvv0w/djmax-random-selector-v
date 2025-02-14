namespace Dmrsv.RandomSelector
{
    public record Pattern
    {
        public int Id { get; init; } = -1;
        public int Level { get; init; } = 0;
        public int TrackId => Id / 100;
        public string ButtonTunes => (Id / 10 % 10) switch
        {
            4 => "4B",
            5 => "5B",
            6 => "6B",
            8 => "8B",
            _ => throw new NotSupportedException("Invalid ButtonTunes")
        };
        public string Difficulty => (Id % 10) switch
        {
            1 => "NM",
            2 => "HD",
            3 => "MX",
            4 => "SC",
            _ => throw new NotSupportedException("Invalid Difficulty")
        };
        public string Style => $"{ButtonTunes}{Difficulty}";

        public Pattern(int id, int level)
        {
            Id = id;
            Level = level;
        }

        public Pattern(int trackId, string style, int level)
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
            Id = trackId * 100 + buttonTunes * 10 + difficulty;
            Level = level;
        }

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
