namespace Dmrsv.RandomSelector
{
    public record Music(string Title, string ButtonTunes, string Difficulty, int Level)
    {
        public string GetStyle() => $"{ButtonTunes}{Difficulty}";
    }
}
