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
using Caliburn.Micro;
using System.Threading;
using DjmaxRandomSelectorV.Models.Interfaces;
using DjmaxRandomSelectorV.Utilities;

namespace DjmaxRandomSelectorV.RandomSelector
{
    public class Selector : IHandle<IFilter>, IHandle<FilterOption>, IHandle<SelectorOption>
    {
        private const string DjmaxTitle = "DJMAX RESPECT V";

        private List<Track> _tracks;
        private List<Music> _musics;
        private List<string> _titles;

        private int _maxExclusionCount;
        private int _inputInterval;
        private bool _savesExclusion;
        private List<string> _exclusions;

        private bool _isUpdated;
        private bool _isRunning;
        private bool _canExecuteWithoutGame;

        private IFilter _filter;
        private ISifter _sifter;
        private IProvider _provider;

        private readonly Executor _executor;

        private readonly IEventAggregator _eventAggregator;

        public List<string> Exclusions
        {
            get { return _savesExclusion ? _exclusions : new List<string>(); }
            set { _exclusions = value; }
        }

        public Selector(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);

            _executor = new Executor(CanStart, Start);
        }

        public void AddHotKey() => _executor.AddHotkey();

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
            _provider?.Provide(music, _tracks, _inputInterval);
        }
        private bool CanStart()
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
        private void Start()
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
            _eventAggregator.PublishOnUIThreadAsync(selectedMusic);
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
        private void ChangeSifter(string filterType)
        {
            string currentMethod = _sifter.CurrentMethod;
            _sifter = filterType switch
            {
                nameof(ConditionalFilter) => new QuerySifter(),
                nameof(SelectiveFilter) => throw new NotImplementedException(),
                _ => throw new NotSupportedException(),
            };
            _sifter.SetMethod(currentMethod);
        }
        private void ChangeProvider(Mode mode, Aider aider)
        {
            bool isFreestyle = mode == Mode.Freestyle;
            _provider = aider switch
            {
                Aider.Off => new Locator(false),
                Aider.AutoStart => new Locator(isFreestyle),
                Aider.Observe => null,
                _ => throw new NotSupportedException(),
            };
        }

        public Task HandleAsync(IFilter message, CancellationToken cancellationToken)
        {
            _filter = message;
            _isUpdated = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(FilterOption message, CancellationToken cancellationToken)
        {
            _maxExclusionCount = message.Except;
            _sifter.ChangeMethod(message);
            ChangeProvider(message.Mode, message.Aider);
            _canExecuteWithoutGame = message.Aider == Aider.Observe;
            _isUpdated = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SelectorOption message, CancellationToken cancellationToken)
        {
            ChangeSifter(message.FilterType);
            _inputInterval = message.InputInterval;
            _savesExclusion = message.SavesExclusion;
            UpdateTrackList(message.OwnedDlcs);
            _isUpdated = true;
            return Task.CompletedTask;
        }
        #endregion

        private void UpdateTrackList(List<string> ownedDlcs)
        {
            var allTrackList = FileManager.GetAllTrackList();
            var categories = ownedDlcs.Concat(new List<string>() { "RP", "P1", "P2", "GG" });
            var titleFilter = CreateTitleFilter(ownedDlcs);

            var trackQuery = from track in allTrackList
                            where categories.Contains(track.Category)
                            && !titleFilter.Contains(track.Title)
                            select track;

            _tracks = trackQuery.ToList();
        }
        private List<string> CreateTitleFilter(List<string> ownedDlcs)
        {
            var list = new List<string>();

            if (!ownedDlcs.Contains("P3"))
            {
                list.Add("glory day (Mintorment Remix)");
                list.Add("glory day -JHS Remix-");
            }
            if (!ownedDlcs.Contains("TR"))
                list.Add("Nevermind");
            if (!ownedDlcs.Contains("CE"))
                list.Add("Rising The Sonic");
            if (!ownedDlcs.Contains("BS"))
                list.Add("ANALYS");
            if (!ownedDlcs.Contains("T1"))
                list.Add("Do you want it");
            if (!ownedDlcs.Contains("T2"))
                list.Add("End of Mythology");
            if (!ownedDlcs.Contains("T3"))
                list.Add("ALiCE");
            if (!ownedDlcs.Contains("TQ"))
                list.Add("Techno Racer");
            if (ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Here in the Moment ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Airwave ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && ownedDlcs.Contains("T1"))
                list.Add("SON OF SUN ~Extended Mix~");
            if (!ownedDlcs.Contains("VE") && ownedDlcs.Contains("VE2"))
                list.Add("너로피어오라 ~Original Ver.~");

            return list;
        }
    }
}
