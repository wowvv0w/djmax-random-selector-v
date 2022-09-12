using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.RandomSelector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DjmaxRandomSelectorV.Models.Interfaces;
using System.Reflection;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public class QuerySifter : ISifter
    {
        private delegate List<Music> SiftingMethod(IEnumerable<Track> tracks, Predicate<KeyValuePair<string, int>> satisfies);
        private SiftingMethod _method;

        public string CurrentMethod
        {
            get { return _method.Method.Name; }
        }
        public void ChangeMethod(FilterOption filterOption)
        {
            if (filterOption.Mode == Mode.Freestyle)
            {
                _method = filterOption.Level switch
                {
                    Level.Off => SiftAll,
                    Level.Beginner => SiftEasiest,
                    Level.Master => SiftHardest,
                    _ => throw new NotSupportedException(),
                };
            }
            else if (filterOption.Mode == Mode.Online)
            {
                _method = SiftAllAsFree;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public void SetMethod(string methodName)
        {
            _method = (SiftingMethod)Delegate.CreateDelegate(typeof(SiftingMethod), this, methodName);
        }
        private List<Music> SiftAll(IEnumerable<Track> tracks, Predicate<KeyValuePair<string, int>> satisfies)
        {
            var musics = from track in tracks
                         from pattern in track.Patterns
                         where satisfies.Invoke(pattern)
                         select new Music
                         {
                             Title = track.Title,
                             Style = pattern.Key,
                             Level = pattern.Value.ToString()
                         };
            return musics.ToList();
        }
        private List<Music> SiftEasiest(IEnumerable<Track> tracks, Predicate<KeyValuePair<string, int>> satisfies)
        {
            var musics = from track in tracks
                         from pattern in track.Patterns
                         where satisfies.Invoke(pattern)
                         group new Music
                         {
                             Title = track.Title,
                             Style = pattern.Key,
                             Level = pattern.Value.ToString()
                         } by new { title = track.Title, button = pattern.Key[..2] } into buttonGroup
                         select buttonGroup.First();
            return musics.ToList();
        }
        private List<Music> SiftHardest(IEnumerable<Track> tracks, Predicate<KeyValuePair<string, int>> satisfies)
        {
            var musics = from track in tracks
                         from pattern in track.Patterns
                         where satisfies.Invoke(pattern)
                         group new Music
                         {
                             Title = track.Title,
                             Style = pattern.Key,
                             Level = pattern.Value.ToString()
                         } by new { title = track.Title, button = pattern.Key[..2] } into buttonGroup
                         select buttonGroup.Last();
            return musics.ToList();
        }
        private List<Music> SiftAllAsFree(IEnumerable<Track> tracks, Predicate<KeyValuePair<string, int>> satisfies)
        {
            var musics = from track in tracks
                         where track.Patterns.Any(pattern => satisfies.Invoke(pattern))
                         select new Music
                         {
                             Title = track.Title,
                             Style = "FREE",
                             Level = "-"
                         };
            return musics.ToList();
        }

        public List<Music> Sift(List<Track> tracks, IFilter filterToConvert)
        {
            var filter = filterToConvert as ConditionalFilter;

            var styles = new List<string>();
            foreach (string button in filter.ButtonTunes)
                foreach (string difficulty in filter.Difficulties)
                    styles.Add($"{button}{difficulty}");

            var siftedTracks = from track in tracks
                               where filter.Categories.Contains(track.Category)
                               || filter.IncludesFavorite && filter.Favorites.Contains(track.Title)
                               select track;

            int minLevel = filter.Levels[0], maxLevel = filter.Levels[1];
            int minSCLevel = filter.ScLevels[0], maxSCLevel = filter.ScLevels[1];
            bool Satisfies(KeyValuePair<string, int> pattern)
            {
                string key = pattern.Key;
                int value = pattern.Value;
                return styles.Contains(key)
                    && ((!key.EndsWith("SC") && value >= minLevel && value <= maxLevel)
                        || (key.EndsWith("SC") && value >= minSCLevel && value <= maxSCLevel));
            }

            return _method(siftedTracks, Satisfies);
        }
    }
}
