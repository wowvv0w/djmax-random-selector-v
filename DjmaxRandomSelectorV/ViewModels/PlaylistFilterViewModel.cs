using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PlaylistFilterViewModel : Screen
    {
        private const string DefaultPath = @"Data\CurrentPlaylist.json";

        private readonly IEventAggregator _eventAggregator;
        private readonly IFileManager _fileManager;
        private readonly List<Track> _tracks;
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

        public PlaylistFilterViewModel(IEventAggregator eventAggregator, IFileManager fileManager)
        {
            DisplayName = "FILTER";
            _eventAggregator = eventAggregator;
            _fileManager = fileManager;

            _filter = _fileManager.Import<PlaylistFilter>(DefaultPath);
            _tracks = new TrackManager().GetAllTrack();
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
            }
            return Task.CompletedTask;
        }

        public void AddItem(Music item)
        {
            PlaylistItems.Add(item with { Level = -1 });
        }

        public void RemoveItem(Music item)
        {
            PlaylistItems.Remove(item);
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
                var concat = _fileManager.Import<PlaylistFilter>(dialog.FileName).Items;
                PlaylistItems.AddRange(concat);
            }
        }

        public void ClearItems()
        {
            var result = MessageBox.Show("Are you sure?",
                "Clear all", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PlaylistItems.Clear();
            }
        }

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
                string fileName = dialog.FileName;
                var playlist = _fileManager.Import<PlaylistFilter>(fileName).Items;
                PlaylistItems.Clear();
                PlaylistItems.AddRange(playlist);
            }
        }

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

        public void SortPlaylist()
        {
            var difficultyOrder = new string[] { "NM", "HD", "MX", "SC" };
            var sorted = PlaylistItems.OrderBy(x => !Regex.IsMatch(x.Title[..1], "[ㄱ-ㅎ가-힣]"))
                .ThenBy(x => x.Title)
                .ThenBy(x => x.ButtonTunes)
                .ThenBy(x => Array.IndexOf(difficultyOrder, x.Difficulty)).ToList();
            PlaylistItems.Clear();
            PlaylistItems.AddRange(sorted);
        }
    }
}
