namespace Dmrsv.RandomSelector
{
    public class OutputMethodCreator
    {
        public Func<IEnumerable<Music>, IEnumerable<Music>>? Create(MusicForm form, LevelPreference preference)
        {
            return (form, preference) switch
            {
                (MusicForm.Free, _) => (IEnumerable<Music> musicList) =>
                {
                    var result = from m in musicList
                                 group m by new { m.Title } into g
                                 select g.First() with
                                 {
                                     ButtonTunes = string.Empty,
                                     Difficulty = string.Empty,
                                     Level = -1
                                 };

                    return result;
                }
                ,
                (MusicForm.Default, LevelPreference.Lowest) => (IEnumerable<Music> musicList) =>
                {
                    var result = from m in musicList
                                 group m by new { m.Title, m.ButtonTunes } into g
                                 select g.First();
                    return result;
                }
                ,
                (MusicForm.Default, LevelPreference.Highest) => (IEnumerable<Music> musicList) =>
                {
                    var result = from m in musicList
                                 group m by new { m.Title, m.ButtonTunes } into g
                                 select g.Last();
                    return result;
                }
                ,
                _ => null
            };
        }
    }
}
