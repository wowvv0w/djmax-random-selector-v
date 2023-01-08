namespace Dmrsv.RandomSelector
{
    public record Track
    {
        public string Title { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public Dictionary<string, int> Patterns { get; init; } = new();

        public IEnumerable<Music> GetMusicList()
        {
            var musicList = from p in Patterns
                            select new Music()
                            {
                                Title = Title,
                                ButtonTunes = p.Key[..2],
                                Difficulty = p.Key[2..4],
                                Level = p.Value
                            };

            return musicList;
        }
    }
}
