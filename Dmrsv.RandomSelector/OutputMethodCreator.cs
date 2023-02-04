namespace Dmrsv.RandomSelector
{
    public class OutputMethodCreator
    {
        public OutputMethodCallback? Create(MusicForm form, LevelPreference preference)
        {
            return (form, preference) switch
            {
                (MusicForm.Free, _) => (musicList) =>
                {
                    var result = from m in musicList
                                 group m by new { m.Title } into g
                                 select g.First() with
                                 {
                                     Style = string.Empty,
                                     Level = -1,
                                 };

                    return result;
                }
                ,
                (MusicForm.Default, LevelPreference.Lowest) => (musicList) =>
                {
                    var result = from m in musicList
                                 group m by new { m.Title, m.ButtonTunes } into g
                                 select g.First();
                    return result;
                }
                ,
                (MusicForm.Default, LevelPreference.Highest) => (musicList) =>
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
