using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using Microsoft.Win32;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class AdvancedFilterViewModel : Screen, IHandle<VArchiveMessage>
    {
        private const string DefaultPath = @"DMRSV3_Data\CurrentPlaylist.json";
        private const string PresetPath = @"DMRSV3_Data\Preset\Playlist";

        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;
        private readonly TrackDB _db;

        private AdvancedFilter _filter;
        private string _searchBox;
        private List<string> _namesake;
        private bool _isWizardOpen;

        public string SearchBox
        {
            get => _searchBox;
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange();
                UpdateResult();
            }
        }

        public BindableCollection<string> TitleSuggestions { get; }
        public BindableCollection<Pattern> SearchResult { get; }
        public BindableCollection<PlaylistItem> PlaylistItems { get; }

        public AdvancedFilterViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, IFileManager fileManager)
        {
            DisplayName = "FILTER";
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
            _windowManager = windowManager;
            _fileManager = fileManager;
            _db = IoC.Get<TrackDB>();
            _namesake = _db.AllTrack
                           .GroupBy(t => t.Title)
                           .Where(g => g.Count() > 1)
                           .SelectMany(g => g, (g, t) => t.Title)
                           .ToList();
            _isWizardOpen = false;

            _filter = new AdvancedFilter();
            PlaylistItems = new BindableCollection<PlaylistItem>();
            try
            {
                // TODO: ddaembbang
                var playlist = _fileManager.Import<AdvancedFilter>(DefaultPath);
                AddToFilterAndPlaylist(playlist.PatternList.Select(x => x.PatternId).ToArray());
            }
            catch
            {
                MessageBox.Show("Failed to load previous filter.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            _eventAggregator.PublishOnUIThreadAsync(new FilterMessage(_filter));

            SearchResult = new BindableCollection<Pattern>();
            TitleSuggestions = new BindableCollection<string>();
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                _fileManager.Export(_filter, DefaultPath);
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Title == "V-ARCHIVE Wizard")
                    {
                        window.Close();
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public void AddItem(Pattern pattern)
        {
            _filter.PatternList.Add(pattern);
            PlaylistItems.Add(new PlaylistItem
            {
                PatternId = pattern.PatternId,
                Title = _namesake.Contains(pattern.Info.Title)
                        ? $"{pattern.Info.Title} ({pattern.Info.Composer})"
                        : pattern.Info.Title,
                Composer = pattern.Info.Composer,
                Category = pattern.Info.Category,
                Style = pattern.Style,
                Level = pattern.Level.ToString()
            });
        }

        private void AddToFilterAndPlaylist(int[] items)
        {
            if (items is null)
            {
                return;
            }

            foreach (int item in items)
            {
                Track track;
                try
                {
                    track = _db.AllTrack[item / 100];
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }

                var pattern = track.Patterns.FirstOrDefault(p => p.PatternId == item);
                if (pattern is null)
                {
                    continue;
                }

                _filter.PatternList.Add(pattern);
                PlaylistItems.Add(new PlaylistItem
                {
                    PatternId = item,
                    Title = _namesake.Contains(track.Title)
                            ? $"{track.Title} ({track.Composer})"
                            : track.Title,
                    Composer = track.Composer,
                    Category = track.Category,
                    Style = pattern.Style,
                    Level = pattern.Level.ToString()
                });
            }
        }

        #region SELECTED methods
        public void DeselectItems(object sended)
        {
            var items = sended as ICollection<object>;
            items.Clear();
        }

        public void MoveItem(int index, int move)
        {
            int newIndex = index + move;
            if (0 <= newIndex && newIndex <= PlaylistItems.Count - 1)
            {
                PlaylistItems.Move(index, index + move);
            }
        }

        public void RemoveItems(object sended)
        {
            var items = sended as ICollection<object>;
            int requested = items.Count;
            if (requested <= 0)
            {
                return;
            }

            int index = -1;
            var indexesToBeRemoved = new List<int>();
            foreach (var i in PlaylistItems)
            {
                index++;
                if (!items.Any(p => ReferenceEquals(p, i)))
                {
                    continue;
                }
                indexesToBeRemoved.Add(index);
                requested--;
                if (requested == 0)
                {
                    break;
                }
            }

            PlaylistItems.IsNotifying = false;
            indexesToBeRemoved.Reverse();
            indexesToBeRemoved.ForEach(i =>
            {
                _filter.PatternList.RemoveAt(i);
                PlaylistItems.RemoveAt(i);
            });
            PlaylistItems.IsNotifying = true;
            PlaylistItems.Refresh();
            items.Clear();
        }
        #endregion

        #region PLAYLIST methods
        public void SortItems()
        {
            var difficultyOrder = new string[] { "NM", "HD", "MX", "SC" };
            var sorted = PlaylistItems.OrderBy(x => !Regex.IsMatch(x.Title[..1], "[ㄱ-ㅎ가-힣]"))
                .ThenBy(x => x.Title)
                .ThenBy(x => x.Style[..2])
                .ThenBy(x => Array.IndexOf(difficultyOrder, x.Style[2..4])).ToList();
            PlaylistItems.Clear();
            PlaylistItems.AddRange(sorted);
        }

        public void DistinctItems()
        {
            var result = MessageBox.Show("Would you like to remove duplicates?",
                "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var deduplicated = PlaylistItems.Distinct();
                PlaylistItems.Clear();
                PlaylistItems.AddRange(deduplicated);
                _filter.PatternList = new ObservableCollection<Pattern>(_filter.PatternList.Distinct());
            }
        }

        public void ClearItems()
        {
            var result = MessageBox.Show("Would you like to clear all items?",
                "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PlaylistItems.Clear();
                _filter.PatternList.Clear();
            }
        }
        #endregion

        #region TOOL methods
        public void SaveItems()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, PresetPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var dialog = new SaveFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string fileName = dialog.FileName;
                var playlist = new Playlist()
                {
                    Items = _filter.PatternList.Select(p => p.PatternId).ToArray()
                };
                _fileManager.Export(playlist, fileName);
            }
        }

        public void LoadItems()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, PresetPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Playlist playlist;
                try
                {
                    playlist = _fileManager.Import<Playlist>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot load the playlist.\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                PlaylistItems.Clear();
                _filter.PatternList.Clear();
                AddToFilterAndPlaylist(playlist.Items);
            }
        }

        public void ConcatenateItems()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, PresetPath);
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Playlist playlist;
                try
                {
                    playlist = _fileManager.Import<Playlist>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot concatenate the items of playlist.\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                AddToFilterAndPlaylist(playlist.Items);
            }
        }
        #endregion

        #region SEARCH methods
        public void ClearSearchBox()
        {
            SearchBox = string.Empty;
        }

        public void SelectSuggestion(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return;
            }
            SearchBox = title;
        }

        public void SearchTitle()
        {
            TitleSuggestions.Clear();

            if (string.IsNullOrEmpty(SearchBox))
            {
                return;
            }

            var titles = from track in _db.AllTrack
                         let title = track.Title
                         select _namesake.Contains(title) ? $"{title} ({track.Composer})" : title;
            if (SearchBox.Equals("#"))
            {
                titles = titles.Where(title => !Regex.IsMatch(title[..1], "[a-z]", RegexOptions.IgnoreCase));
            }
            else
            {
                titles = titles.Where(title => title.StartsWith(SearchBox, true, null));
            }
            TitleSuggestions.AddRange(titles);
        }

        private void UpdateResult()
        {
            SearchResult.Clear();
            var track = _namesake.Any(title => SearchBox.StartsWith(title))
                        ? _db.AllTrack.FirstOrDefault(t => SearchBox.Equals($"{t.Title} ({t.Composer})"), null)
                        : _db.AllTrack.FirstOrDefault(t => t.Title == SearchBox, null);
            if (track is null)
            {
                return;
            }
            var buttonTunes = new string[] { "4B", "5B", "6B", "8B" };
            var difficulty = new string[] { "NM", "HD", "MX", "SC" };
            var result = from bt in buttonTunes
                         from df in difficulty
                         select track.Patterns.FirstOrDefault(p => p.Style == $"{bt}{df}", null);
            SearchResult.AddRange(result);
        }
        #endregion

        public Task RunVArchiveWizard()
        {
            if (_isWizardOpen)
            {
                return Task.CompletedTask;
            }
            _isWizardOpen = true;
            return _windowManager.ShowWindowAsync(IoC.Get<VArchiveWizardViewModel>());
        }

        public Task HandleAsync(VArchiveMessage message, CancellationToken cancellationToken)
        {
            if (message.Command == "close")
            {
                _isWizardOpen = false;
                return Task.CompletedTask;
            }
            if (message.Command == "overwrite" && PlaylistItems.Any())
            {
                var result = MessageBox.Show("The playlist will be overwrited.\nContinue?",
                    "Overwrite Playlist", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PlaylistItems.Clear();
                    _filter.PatternList.Clear();
                }
                else
                {
                    return Task.CompletedTask;
                }
            }
            AddToFilterAndPlaylist(message.Items);
            return Task.CompletedTask;
        }
    }
}
