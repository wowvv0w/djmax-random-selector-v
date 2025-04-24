namespace Dmrsv.RandomSelector.Enums
{
    public enum ButtonTunes
    {
        Four = 4,
        Five = 5,
        Six = 6,
        Eight = 8
    }

    public static class ButtonTunesExtensions
    {
        public static string AsString(this ButtonTunes buttonTunes) => buttonTunes switch
        {
            ButtonTunes.Four => "4B",
            ButtonTunes.Five => "5B",
            ButtonTunes.Six => "6B",
            ButtonTunes.Eight => "8B",
            _ => throw new NotSupportedException("Invalid ButtonTunes")
        };

        public static ButtonTunes AsButtonTunes(this string str) => str switch
        {
            "4B" => ButtonTunes.Four,
            "5B" => ButtonTunes.Five,
            "6B" => ButtonTunes.Six,
            "8B" => ButtonTunes.Eight,
            _ => throw new NotSupportedException("Invalid ButtonTunes")
        };
    }
}
