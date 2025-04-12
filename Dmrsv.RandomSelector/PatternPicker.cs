namespace Dmrsv.RandomSelector
{
    public class PatternPicker
    {
        private Func<IEnumerable<Pattern>, IEnumerable<Pattern>>? pickMethod;

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
                (MusicForm.Free, _) => (patternList) =>
                {
                    return from p in patternList
                           group p by new { p.TrackId } into g
                           select g.First();
                }
                ,
                (MusicForm.Default, LevelPreference.Lowest) => (patternList) =>
                {
                    return from p in patternList
                           group p by new { p.TrackId, p.Button } into g
                           select g.First();
                }
                ,
                (MusicForm.Default, LevelPreference.Highest) => (patternList) =>
                {
                    return from p in patternList
                           group p by new { p.TrackId, p.Button } into g
                           select g.Last();
                }
                ,
                _ => null
            };
        }
    }
}
