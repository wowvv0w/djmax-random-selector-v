using Dmrsv.Data.Enums;
using Dmrsv.Data.DataTypes;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Dmrsv.RandomSelector.Sifters;
using Dmrsv.RandomSelector.Providers;
using Dmrsv.Data.Controller;

namespace Dmrsv.RandomSelector
{
    public class Selector
    {
        private const string DjmaxTitle = "DJMAX RESPECT V";

        private List<Track>? _tracks;
        private List<Music>? _musics;
        private List<string>? _titles;

        private int _maxExclusionCount;
        private int _inputInterval;
        private List<string> _exclusions;

        private bool _isUpdated;
        private bool _isRunning;
        private bool _canExecuteWithoutGame;

        private IFilter? _filter;
        private ISifter? _sifter;
        private IProvider? _provider;

        private Action<Music> _publish;

        public Selector(Action<Music> publish)
        {
            UpdateTrackList(new OptionApi().GetSelectorOption().OwnedDlcs);
            _exclusions = new FilterApi().GetExtraFilter().Exclusions;
            _isUpdated = true;
            _isRunning = false;
            Handle(new OptionApi().GetSelectorOption());
            _publish = publish;
        }

        private void UpdateExclusions()
        {
            int exclusionsCount = _exclusions.Count;
            int titleCount = _titles!.Count;
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

        public bool CanStart()
        {
            if (_canExecuteWithoutGame)
                return true;
            else if (!GetActiveWindowTitle().Equals(DjmaxTitle))
            {
                throw new Exception();
            }
            else if (!_isRunning)
                return true;
            else
                return false;
        }

        public void Start()
        {
            _isRunning = true;

            if (_isUpdated)
            {
                _musics = _sifter!.Sift(_tracks!, _filter!);
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

            var filteredList = (from music in _musics
                               where !_exclusions.Contains(music.Title)
                               select music).ToList();

            if (filteredList.Any())
            {
                var random = new Random();
                int index = random.Next(filteredList.Count - 1);
                Music selectedMusic = filteredList[index];

                _provider?.Provide(selectedMusic, _tracks!, _inputInterval);
                _exclusions.Add(selectedMusic.Title);
                _publish.Invoke(selectedMusic);
            }
            else
            {
                throw new Exception();
            }

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
            return string.Empty;
        }

        #region Sifter, Provider
        private void ChangeSifter(string filterType)
        {
            string? currentMethod = _sifter?.CurrentMethod;
            _sifter = filterType switch
            {
                nameof(ConditionalFilter) => new QuerySifter(),
                nameof(SelectiveFilter) => new PlaylistSifter(),
                _ => throw new NotSupportedException(),
            };

            if (!string.IsNullOrEmpty(currentMethod))
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

        public void Handle(IFilter message)
        {
            _filter = message;
            _isUpdated = true;
        }

        public void Handle(FilterOption message)
        {
            _maxExclusionCount = message.Except;
            _sifter?.ChangeMethod(message);
            ChangeProvider(message.Mode, message.Aider);
            _canExecuteWithoutGame = message.Aider == Aider.Observe;
            _isUpdated = true;
        }

        public void Handle(SelectorOption message)
        {
            ChangeSifter(message.FilterType);
            _inputInterval = message.InputInterval;
            UpdateTrackList(message.OwnedDlcs);
            _isUpdated = true;
        }
        #endregion

        private void UpdateTrackList(List<string> ownedDlcs)
        {
            var allTrackList = new TrackApi().GetAllTrackList();
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
