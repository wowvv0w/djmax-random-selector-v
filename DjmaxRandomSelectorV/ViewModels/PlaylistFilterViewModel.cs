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
        private bool _searchesSuggestion;

        public BindableCollection<string> TitleSuggestions { get; }
        public BindableCollection<Music> SearchResult { get; }
        public BindableCollection<Music> PlaylistItems { get; }

        public PlaylistFilterViewModel(IEventAggregator eventAggregator, IFileManager fileManager)
        {
            DisplayName = "FILTER";
            _eventAggregator = eventAggregator;
            _fileManager = fileManager;
            _searchesSuggestion = true;

            _filter = _fileManager.Import<PlaylistFilter>(DefaultPath);
            _tracks = new TrackManager().GetAllTrack();
            _titles = _tracks.ConvertAll(x => x.Title);

            PlaylistItems = new BindableCollection<Music>(_filter.Items);
            _filter.Items = PlaylistItems;
            _eventAggregator.PublishOnUIThreadAsync(new FilterMessage(_filter));

            SearchResult = new BindableCollection<Music>();
            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;
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
            PlaylistItems.Clear();
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

        #region SearchBox
        private string _searchBox;
        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange();
                if (_searchesSuggestion)
                { 
                    SearchTitle();
                }
                UpdateResult();
            }
        }

        private void SearchTitle()
        {
            TitleSuggestions.Clear();

            if (string.IsNullOrEmpty(_searchBox))
            {
                OpensSuggestionBox = false;
                return;
            }

            OpensSuggestionBox = true;
            
            if (_searchBox.Equals("#"))
            {
                var titles = from title in _titles
                             where !Regex.IsMatch(title, "[a-z]", RegexOptions.IgnoreCase)
                             select title;
                TitleSuggestions.AddRange(titles);
            }
            else
            {
                var titles = from title in _titles
                             where title.StartsWith(_searchBox, true, null)
                             select title;
                TitleSuggestions.AddRange(titles);
            }

            if (TitleSuggestions.Count == 0)
            {
                OpensSuggestionBox = false;
            }
        }
        #endregion

        #region SuggestionBox
        private bool _opensSuggestionBox;
        public bool OpensSuggestionBox
        {
            get { return _opensSuggestionBox; }
            set
            {
                _opensSuggestionBox = value;
                NotifyOfPropertyChange(() => OpensSuggestionBox);
            }
        }

        public void SelectSuggestion(string title)
        {
            _searchesSuggestion = false;

            SearchBox = title;
            OpensSuggestionBox = false;

            _searchesSuggestion = true;
        }

        private void UpdateResult()
        {
            SearchResult.Clear();
            var query = from t in _tracks
                        where t.Title.ToLower() == (_searchBox?.ToLower() ?? string.Empty)
                        select t.GetMusicList() into musicList
                        from m in musicList
                        select m;
            SearchResult.AddRange(query);
        }
        #endregion
    }
}
