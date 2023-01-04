using Dmrsv.Data;
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

namespace Dmrsv.RandomSelector
{
    public class Selector
    {
        private readonly IHub _hub;

        private List<Track> _tracks;
        private List<Music> _musics;
        private List<string> _titles;

        private int _maxExclusionCount;
        private int _inputInterval;
        private List<string> _exclusions;

        private bool _isUpdated;
        private bool _isRunning;
        private bool _canExecuteWithoutGame;

        private IFilter? _filter;
        private ISifter? _sifter;
        private IProvider? _provider;

        public Selector(IHub hub)
        {
            _hub = hub;
            _tracks = new List<Track>();
            _musics = new List<Music>();
            _titles = new List<string>();
            _exclusions = new List<string>();
            _isUpdated = true;
            _isRunning = false;
            //Handle(new OptionApi().GetSelectorOption());
        }

        public void LoadConfiguration(Configuration config)
        {
            UpdateTracks(config.OwnedDlcs);
            _exclusions = config.Exclusions;
            _isUpdated = true;
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

        public bool CanStart()
        {
            if (_canExecuteWithoutGame)
                return true;
            else if (!new WindowTitleHelper().EqualsDjmax())
                throw new Exception("The foreground window is not \"DJMAX RESPECT V\".\nPress start key in the game.");
            else if (!_isRunning)
                return true;
            else
                return false;
        }

        public Music Start()
        {
            _isRunning = true;

            if (_isUpdated)
            {
                _musics = _sifter!.Sift(_tracks, _filter!);
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

            Music selectedMusic;
            if (filteredList.Any())
            {
                var random = new Random();
                int index = random.Next(filteredList.Count - 1);
                selectedMusic = filteredList[index];

                _provider?.Provide(selectedMusic, _tracks, _inputInterval);
                _exclusions.Add(selectedMusic.Title);
            }
            else
            {
                throw new Exception("There is no music that meets the filter conditions.");
            }

            _isRunning = false;
            return selectedMusic;
        }

        private void ChangeSifter(FilterType filterType)
        {
            _sifter = filterType switch
            {
                FilterType.Query => new QuerySifter(),
                FilterType.Playlist => new PlaylistSifter(),
                _ => throw new NotSupportedException(),
            };
            Handle(new OptionApi().GetFilterOption());
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
            UpdateTracks(message.OwnedDlcs);
            _isUpdated = true;
        }

        private void UpdateTracks(List<string> ownedDlcs)
        {
            _tracks = new TrackManager().CreateTracks(ownedDlcs);
        }
    }
}
