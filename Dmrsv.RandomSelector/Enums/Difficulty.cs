namespace Dmrsv.RandomSelector.Enums
{
    public enum Difficulty
    {
        Normal,
        Hard,
        Maximum,
        SC
    }

    public static class DifficultyExtensions
    {
        public static string AsString(this Difficulty difficulty) => difficulty switch
        {
            Difficulty.Normal => "NM",
            Difficulty.Hard => "HD",
            Difficulty.Maximum => "MX",
            Difficulty.SC => "SC",
            _ => throw new NotSupportedException("Invalid Difficulty")
        };

        public static Difficulty AsDifficulty(this string str) => str switch
        {
            "NM" => Difficulty.Normal,
            "HD" => Difficulty.Hard,
            "MX" => Difficulty.Maximum,
            "SC" => Difficulty.SC,
            _ => throw new NotSupportedException("Invalid Difficulty")
        };
    }
}
