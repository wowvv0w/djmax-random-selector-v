namespace Dmrsv.RandomSelector
{
    public record Track()
    {
        public string Title { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public Dictionary<string, int> Patterns { get; init; } = new();

        public List<Music> GetMusicList()
        {
            var musicList = from p in Patterns
                            select new Music(Title, p.Key[..2], p.Key[2..4], p.Value);

            return musicList.ToList();
        }
    }
}
