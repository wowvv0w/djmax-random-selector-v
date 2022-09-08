using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.RandomSelector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace DjmaxRandomSelectorV.RandomSelector
{
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
