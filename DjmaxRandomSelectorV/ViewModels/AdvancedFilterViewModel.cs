using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class AdvancedFilterViewModel : Screen, IHandle<VArchiveMessage>
    {
        private const string DefaultPath = @"DMRSV3_Data\CurrentPlaylist.json";
        private const string PresetPath = @"DMRSV3_Data\Preset\Playlist";

        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;
        private readonly IReadOnlyList<Track> _allTrack;

        private AdvancedFilter _filter;
        private Track _selectedTrack;
        private string _searchBox;

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
            _allTrack = IoC.Get<TrackDB>().AllTrack;

            _filter = new AdvancedFilter();
            PlaylistItems = new BindableCollection<PlaylistItem>();
            try
            {
                var playlist = _fileManager.Import<Playlist>(DefaultPath);
                AddToFilterAndPlaylist(playlist.Items);
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
                PatternId = pattern.Id,
                Title = _selectedTrack.Title,
                Composer = _selectedTrack.Composer,
                Category = _selectedTrack.Category,
                Style = pattern.Style,
                Level = pattern.Level.ToString()
            });
        }

        private void AddToFilterAndPlaylist(int[] items)
        {
            foreach (int item in items)
            {
                Track track = _allTrack.FirstOrDefault(t => t.EqualsTrackId(item));
                if (track is not null)
                {
                    var pattern = track.GetPatternFromId(item);
                    _filter.PatternList.Add(pattern);
                    PlaylistItems.Add(new PlaylistItem
                    {
                        PatternId = item,
                        Title = track.Title,
                        Composer = track.Composer,
                        Category = track.Category,
                        Style = pattern.Style,
                        Level = pattern.Level.ToString()
                    });
                }
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
                    Items = _filter.PatternList.Select(p => p.Id).ToArray()
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

            IEnumerable<string> titles;
            if (SearchBox.Equals("#"))
            {
                titles = from title in _allTrack.Select(t => t.Title)
                         where !Regex.IsMatch(title[..1], "[a-z]", RegexOptions.IgnoreCase)
                         select title;
            }
            else
            {
                titles = from title in _allTrack.Select(t => t.Title)
                         where title.StartsWith(SearchBox, true, null)
                         select title;
            }
            TitleSuggestions.AddRange(titles);
        }

        private void UpdateResult()
        {
            SearchResult.Clear();
            var query = from t in _allTrack
                        where t.Title.ToLower() == (SearchBox?.ToLower() ?? string.Empty)
                        select t.GetPatterns() into patternList
                        from p in patternList
                        select p;
            SearchResult.AddRange(query);
        }
        #endregion

        public Task RunVArchiveWizard()
        {
            return _windowManager.ShowWindowAsync(IoC.Get<VArchiveWizardViewModel>());
        }

        public Task HandleAsync(VArchiveMessage message, CancellationToken cancellationToken)
        {
            if (message.Command == "overwrite" && PlaylistItems.Any())
            {
                var result = MessageBox.Show("The playlist will be overwrited.\nContinue?",
                    "Overwrite Playlist", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PlaylistItems.Clear();
                    _filter.PatternList.Clear();
                }
            }
            AddToFilterAndPlaylist(message.Items);
            return Task.CompletedTask;
        }
    }
}
