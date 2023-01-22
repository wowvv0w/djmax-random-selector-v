namespace Dmrsv.RandomSelector
{
    public record Track(string Title, string Category, Dictionary<string, int> Patterns)
    {
        public IEnumerable<Music> GetMusicList()
        {
            var musicList = from p in Patterns
                            select new Music(Title, p.Key[..2], p.Key[2..4], p.Value);

            return musicList;
        }
    }
}
