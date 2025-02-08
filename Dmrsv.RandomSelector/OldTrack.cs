namespace Dmrsv.RandomSelector
{
    public record OldTrack
    {
        public string Title { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public Dictionary<string, int> Patterns { get; init; } = new();

        public IEnumerable<Music> GetMusicList()
        {
            var musicList = from p in Patterns
                            select new Music(Title, p.Key, p.Value);

            return musicList;
        }
    }
}
