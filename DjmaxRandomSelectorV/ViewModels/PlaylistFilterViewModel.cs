using Caliburn.Micro;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.Controller;
using Dmrsv.Data.DataTypes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PlaylistFilterViewModel : FilterBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly List<Music> _allItems;
        private readonly List<string> _allTitles;
        private readonly FilterApi _api;

        private bool _searchesSuggestion;

        public BindableCollection<string> TitleSuggestions { get; set; }
        public BindableCollection<Music> SearchResult { get; set; }
        public BindableCollection<Music> PlaylistItems { get; set; }

        public PlaylistFilterViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _searchesSuggestion = true;
            _api = new FilterApi();

            var allTracks = new TrackApi().GetAllTrackList().ToList();
            var playlistItemsQuery = from track in allTracks
                                     from pattern in track.Patterns
                                     where pattern.Value > 0
                                     select new Music() 
                                     { 
                                         Title = track.Title,
                                         Style = pattern.Key, 
                                         Level = pattern.Value.ToString(),
                                     };
            _allItems = playlistItemsQuery.ToList();
            _allTitles = _allItems.Select(x => x.Title).Distinct().ToList();

            List<Music> playlist = _api.GetPlaylistFilter().Playlist;
            PlaylistItems = new BindableCollection<Music>(playlist);
            SearchResult = new BindableCollection<Music>();
            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;

            Publish();
        }
        protected override void Publish()
        {
            var filter = new PlaylistFilter() { Playlist = PlaylistItems.ToList() };
            _api.SetPlaylistFilter(filter);
            _eventAggregator.PublishOnUIThreadAsync(filter);
        }

        public void AddItem(Music item)
        {
            PlaylistItems.Add(item);
            Publish();
        }

        public void RemoveItem(Music item)
        {
            PlaylistItems.Remove(item);
            Publish();
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
                var concat = _api.GetPlaylist(dialog.FileName).Playlist;
                PlaylistItems.AddRange(concat);
            }

            Publish();
        }
        public void ClearItems() => PlaylistItems.Clear();
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
                var selectiveFilter = new PlaylistFilter()
                {
                    Playlist = PlaylistItems.ToList(),
                };
                _api.SetPlaylist(selectiveFilter, dialog.FileName);
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
                List<Music> playlist = _api.GetPlaylist(dialog.FileName).Playlist;
                PlaylistItems.Clear();
                PlaylistItems.AddRange(playlist);
            }

            Publish();
        }

        #region SearchBox
        private string _searchBox;
        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange(() => SearchBox);
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
                var titles = from title in _allTitles
                             where !Regex.IsMatch(title, "[a-z]", RegexOptions.IgnoreCase)
                             select title;
                TitleSuggestions.AddRange(titles);
            }
            else
            {
                var titles = from title in _allTitles
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
            var query = from item in _allItems
                        where item.Title.ToLower().Equals(_searchBox?.ToLower() ?? "")
                        select item;
            SearchResult.AddRange(query);
        }
        #endregion
    }
}
