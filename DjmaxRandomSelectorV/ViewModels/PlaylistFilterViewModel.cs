using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PlaylistFilterViewModel : Screen, IHandle<VArchiveMessage>
    {
        private const string DefaultPath = @"Data\CurrentPlaylist.json";

        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;
        private readonly List<OldTrack> _tracks;
        private readonly List<string> _titles;

        private PlaylistFilter _filter;
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
        public BindableCollection<Music> SearchResult { get; }
        public BindableCollection<Music> PlaylistItems { get; }

        public PlaylistFilterViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, IFileManager fileManager)
        {
            DisplayName = "FILTER";
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _fileManager = fileManager;
            _eventAggregator.SubscribeOnUIThread(this);
            try
            {
                _filter = _fileManager.Import<PlaylistFilter>(DefaultPath);
            }
            catch
            {
                _filter = new PlaylistFilter();
            }
            _tracks = new TrackManager().GetAllTrack().ToList();
            _titles = _tracks.ConvertAll(x => x.Title);

            PlaylistItems = new BindableCollection<Music>(_filter.Items);
            _filter.Items = PlaylistItems;
            _eventAggregator.PublishOnUIThreadAsync(new FilterMessage(_filter));

            SearchResult = new BindableCollection<Music>();
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

        public void AddItem(Music item)
        {
            PlaylistItems.Add(item with { Level = -1 });
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
                if (!items.Any(m => ReferenceEquals(m, i)))
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
            indexesToBeRemoved.ForEach(i => PlaylistItems.RemoveAt(i));
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
                .ThenBy(x => x.ButtonTunes)
                .ThenBy(x => Array.IndexOf(difficultyOrder, x.Difficulty)).ToList();
            PlaylistItems.Clear();
            PlaylistItems.AddRange(sorted);
        }

        public void DistinctItems()
        {
            var result = MessageBox.Show("Would you like to remove duplicates?",
                "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var deduplicated = PlaylistItems.Distinct().ToList();
                PlaylistItems.Clear();
                PlaylistItems.AddRange(deduplicated);
            }
        }

        public void ClearItems()
        {
            var result = MessageBox.Show("Would you like to clear all items?",
                "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PlaylistItems.Clear();
            }
        }
        #endregion

        #region TOOL methods
        public void SaveItems()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, @"Data\Playlist");
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
                _fileManager.Export(_filter, fileName);
            }
        }

        public void LoadItems()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, @"Data\Playlist");
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
                PlaylistFilter filter;
                try
                {
                    filter = _fileManager.Import<PlaylistFilter>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot load the playlist.\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var items = filter.Items;
                PlaylistItems.Clear();
                PlaylistItems.AddRange(items);
            }
        }

        public void ConcatenateItems()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, @"Data\Playlist");
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                PlaylistFilter filter;
                try
                {
                    filter = _fileManager.Import<PlaylistFilter>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot concatenate the items of playlist.\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var concat = filter.Items;
                PlaylistItems.AddRange(concat);
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
                titles = from title in _titles
                         where !Regex.IsMatch(title[..1], "[a-z]", RegexOptions.IgnoreCase)
                         select title;
            }
            else
            {
                titles = from title in _titles
                         where title.StartsWith(SearchBox, true, null)
                         select title;
            }
            TitleSuggestions.AddRange(titles);
        }

        private void UpdateResult()
        {
            SearchResult.Clear();
            var query = from t in _tracks
                        where t.Title.ToLower() == (SearchBox?.ToLower() ?? string.Empty)
                        select t.GetMusicList() into musicList
                        from m in musicList
                        select m;
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
                }
            }
            PlaylistItems.AddRange(message.Items);
            return Task.CompletedTask;
        }
    }
}
