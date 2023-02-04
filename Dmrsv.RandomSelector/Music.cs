using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public record Music
    {
        public string Title { get; init; } = string.Empty;
        public string Style { get; init; } = string.Empty;
        [JsonIgnore]
        public int Level { get; init; } = 0;

        public string ButtonTunes { get => Style[..2]; }
        public string Difficulty { get => Style[2..4]; }
        
        public Music(string title, string style, int level)
        {
            Title = title;
            Style = style;
            Level = level;
        }
    }
}
