namespace Dmrsv.RandomSelector
{
    public class PatternPicker
    {
        public Func<IEnumerable<Pattern>, IEnumerable<Pattern>>? pickMethod;

        public PatternPicker()
        {
            pickMethod = null;
        }

        public IEnumerable<Pattern> Pick(IEnumerable<Pattern> patternList)
        {
            return pickMethod?.Invoke(patternList) ?? patternList;
        }

        public void SetPickMethod(MusicForm form, LevelPreference preference)
        {
            pickMethod = (form, preference) switch
            {
                (MusicForm.Free, _) => (musicList) =>
                {
                    return from m in musicList
                           group m by new { m.Id } into g
                           select g.First();
                },
                (MusicForm.Default, LevelPreference.Lowest) => (musicList) =>
                {
                    return from m in musicList
                           group m by new { m.Id, m.ButtonTunes } into g
                           select g.First();
                },
                (MusicForm.Default, LevelPreference.Highest) => (musicList) =>
                {
                    return from m in musicList
                           group m by new { m.Id, m.ButtonTunes } into g
                           select g.Last();
                },
                _ => null
            };
        }
    }
}
