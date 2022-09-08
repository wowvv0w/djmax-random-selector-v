using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DjmaxRandomSelectorV.DataTypes.Interfaces;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public abstract class Filter
    {
        public List<string> Exclusions { get; set; } = new();
    }
    public class ConditionalFilter : Filter
    {
        public List<string> ButtonTunes { get; set; } = new() { "4B", "5B", "6B", "8B" };
        public List<string> Difficulties { get; set; } = new() { "NM", "HD", "MX", "SC" };
        public List<string> Categories { get; set; } = new() 
        { 
            "RP", "P1", "P2", "P3", "TR", "CE", "BS", "VE", "VE2", "ES", "T1", "T2", "T3", 
            "TQ", "GG", "CHU", "CY", "DM", "ESTI", "GC", "GF", "MD", "NXN"
        };
        public int[] Levels { get; set; } = { 1, 15 };
        public int[] ScLevels { get; set; } = { 1, 15 };
        public bool IncludesFavorite { get; set; } = false;
        public List<string> Favorites { get; set; } = new();
    }
    public class SelectiveFilter : Filter
    {
        public List<Music> Playlist { get; set; } = new();
    }

    public class Config
    {
        public FilterOption FilterOption { get; set; } = new();
        public SelectorOption SelectorOption { get; set; } = new();
        public double[] Position { get; set; } = null;
        public int AllTrackVersion { get; set; } = 0;
    }
    public class FilterOption
    {
        public int Except { get; set; } = 5;
        public Mode Mode { get; set; } = Mode.Freestyle;
        public Aider Aider { get; set; } = Aider.Off;
        public Level Level { get; set; } = Level.Off;
        public Type FilterType { get; set; } = typeof(ConditionalFilter);
    }
    public class SelectorOption
    {
        public int InputInterval { get; set; } = 30;
        public bool SavesExclusion { get; set; } = false;
        public List<string> OwnedDlcs { get; set; } = new();
    }
    public interface ISifter
    {
        public void ChangeMethod(FilterOption filterOption);
        public List<Music> Sift(List<Track> tracks, Filter filterToConvert);
    }
    public class QuerySifter : ISifter
    {
        private delegate List<Music> SiftingMethod(IEnumerable<Track> tracks, Predicate<KeyValuePair<string, int>> satisfies);
        private SiftingMethod _method;
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
                         } by pattern.Key[..2] into buttonGroup
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
                         } by pattern.Key[..2] into buttonGroup
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

        public List<Music> Sift(List<Track> tracks, Filter filterToConvert)
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
    public class Selector
    {
        private const string DjmaxTitle = "DJMAX RESPECT V";

        private const string ReleasesUrl = "https://github.com/wowvv0w/djmax-random-selector-v/releases";
        private const string VersionsUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Version.txt";
        

        private const string ConfigPath = "Data/Config.json";
        private const string CurrentFilterPath = "Data/CurrentFilter.json";
        private const string CurrentPlaylistPath = "Data/CurrentPlaylist.json";

        private List<Track> _tracks;
        private List<Music> _musics;
        private List<string> _titles;

        private int _maxExclusionCount;
        private int _inputInterval;
        private List<string> _exclusions;

        private bool _isUpdated;
        private bool _isRunning;
        private bool _canExecuteWithoutGame;

        private ISifter _sifter;
        private IProvider _provider;

        private readonly Executor _executor;
        private readonly TrackHandler _trackHandler;

        private Filter _filter;

        public Selector()
        {
            _executor = new Executor(CanStart, Start);
            _trackHandler = new TrackHandler();
        }

        private void UpdateExclusions()
        {
            int exclusionsCount = _exclusions.Count;
            int titleCount = _titles.Count;
            int maxCount = _maxExclusionCount;
            if (exclusionsCount > maxCount)
            {
                _exclusions.RemoveRange(0, exclusionsCount - maxCount);
            }
            else if (exclusionsCount >= titleCount)
            {
                try
                {
                    _exclusions.RemoveRange(0, exclusionsCount - titleCount + 1);
                }
                catch (ArgumentException)
                {
                }
            }
        }
        private void Sift()
        {
            _musics = _sifter.Sift(_tracks, _filter);
        }
        private Music Pick(List<Music> musicList)
        {
            var random = new Random();
            var index = random.Next(musicList.Count - 1);
            var selectedMusic = musicList[index];

            return selectedMusic;
        }
        private void Provide(Music music)
        {
            _provider.Provide(music, _tracks, _inputInterval);
        }
        public bool CanStart()
        {
            if (_canExecuteWithoutGame)
                return true;
            else if (!GetActiveWindowTitle().Equals(DjmaxTitle))
            {
                MessageBox.Show("Foreground window is not DJMAX RESPECT V.",
                    "Selector Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!_isRunning)
                return true;
            else
                return false;
        }
        public void Start()
        {
            _isRunning = true;

            Music selectedMusic;
            if (_isUpdated)
            {
                Sift();
                _exclusions.Clear();
                var titleList = from music in _musics
                                select music.Title;
                _titles = titleList.Distinct().ToList();
                _isUpdated = false;
            }
            else
            {
                UpdateExclusions();
            }

            var filteredList = from music in _musics
                            where !_exclusions.Contains(music.Title)
                            select music;
            if (filteredList.Any())
            {
                selectedMusic = Pick(filteredList.ToList());
            }
            else
            {
                MessageBox.Show("There is no music in filtered list.",
                    "Filter Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _isRunning = false;
                return;
            }

            Provide(selectedMusic);
            _exclusions.Add(selectedMusic.Title);
            _isRunning = false;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        #region Sifter, Provider
        private void ChangeSifter()
        {
            _sifter = _filter switch
            {
                ConditionalFilter => new QuerySifter(),
                SelectiveFilter => throw new NotImplementedException(),
                _ => throw new NotSupportedException(),
            };
        }
        private void ChangeProvider(Mode mode, Aider aider)
        {
            _provider = aider switch
            {
                Aider.Off => new Locator(true),
                Aider.AutoStart => mode == Mode.Freestyle ? new Locator(true) : new Locator(false),
                Aider.Observe => null,
                _ => throw new NotSupportedException(),
            };
        }
        #endregion
    }
}
