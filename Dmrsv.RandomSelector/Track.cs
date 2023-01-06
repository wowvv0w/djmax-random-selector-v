namespace Dmrsv.RandomSelector
{
    public record Track
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, int> Patterns { get; set; } = new();

        public IEnumerable<Music> GetMusicList()
        {
            var musicList = from p in Patterns
                            select new Music()
                            {
                                Title = Title,
                                Style = p.Key,
                                Level = p.Value
                            };

            return musicList;
        }
    }
}
