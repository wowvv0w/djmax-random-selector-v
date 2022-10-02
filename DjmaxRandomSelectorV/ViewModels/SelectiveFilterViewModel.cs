using Caliburn.Micro;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.Controller;
using Dmrsv.Data.DataTypes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SelectiveFilterViewModel : FilterBaseViewModel
    {
        private readonly List<PlaylistItem> _allPlaylistItems;
        private readonly List<string> _allTitles;
        private readonly FilterApi _api;

        private bool _searchesSuggestion;
        public BindableCollection<string> TitleSuggestions { get; set; }
        public BindableCollection<PlaylistItem> PlaylistItems { get; set; }

        private readonly IEventAggregator _eventAggregator;
        public SelectiveFilterViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _searchesSuggestion = true;
            _api = new FilterApi();

            var allTracks = new TrackApi().GetAllTrackList().ToList();
            var playlistItemsQuery = from track in allTracks
                                     from pattern in track.Patterns
                                     where pattern.Value > 0
                                     select new PlaylistItem(track.Title, pattern.Key);
            _allPlaylistItems = playlistItemsQuery.ToList();
            _allTitles = _allPlaylistItems.Select(x => x.Title).Distinct().ToList();

            List<PlaylistItem> playlist = _api.GetSelectiveFilter().Playlist;
            PlaylistItems = new BindableCollection<PlaylistItem>(playlist);
            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;

            Publish();
        }
        protected override void Publish()
        {
            var filter = new SelectiveFilter() { Playlist = PlaylistItems.ToList() };
            _api.SetSelectiveFilter(filter);
            _eventAggregator.PublishOnUIThreadAsync(filter);
        }

        public void AddItem()
        {
            PlaylistItem item = _allPlaylistItems.Find(x => x.Title.Equals(TitleSearchBox) && x.Style.Equals(StyleSearchBox.ToUpper()));
            if (item.Title != null || item.Style != null)
            {
                PlaylistItems.Add(item);
                TitleSearchBox = string.Empty;
                StyleSearchBox = string.Empty;
                Publish();
            }
            else
            {
                MessageBox.Show("No such music found.", "Filter Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void RemoveItem(object item)
        {
            PlaylistItems.Remove((PlaylistItem)item);
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
                var selectiveFilter = new SelectiveFilter()
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
                List<PlaylistItem> playlist = _api.GetPlaylist(dialog.FileName).Playlist;
                PlaylistItems.Clear();
                PlaylistItems.AddRange(playlist);
            }

            Publish();
        }

        #region SearchBox
        private string _titleSearchBox;
        private string _styleSearchBox;
        public string TitleSearchBox
        {
            get { return _titleSearchBox; }
            set
            {
                _titleSearchBox = value;
                NotifyOfPropertyChange(() => TitleSearchBox);
                if (_searchesSuggestion) { SearchTitle(); }
            }
        }
        public string StyleSearchBox
        {
            get { return _styleSearchBox; }
            set
            {
                _styleSearchBox = value;
                NotifyOfPropertyChange(() => StyleSearchBox);
            }
        }

        private void SearchTitle()
        {
            TitleSuggestions.Clear();

            if (string.IsNullOrEmpty(TitleSearchBox))
            {
                OpensSuggestionBox = false;
                return;
            }

            OpensSuggestionBox = true;
            
            var titles = from title in _allTitles
                         where title.StartsWith(TitleSearchBox, true, null)
                         select title;
            titles = titles.Count() < 8 ? titles.Take(titles.Count()) : titles.Take(8);
            TitleSuggestions.AddRange(titles);

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

            TitleSearchBox = title;
            OpensSuggestionBox = false;

            _searchesSuggestion = true;
        }
        #endregion
    }
}
