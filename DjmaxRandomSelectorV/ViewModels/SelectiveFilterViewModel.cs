using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Models.Interfaces;
using DjmaxRandomSelectorV.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SelectiveFilterViewModel : FilterBaseViewModel
    {
        private const string CurrentFilterPath = "Data/CurrentPlaylist.json";

        private readonly List<Track> _allTracks;

        private bool _searchesSuggestion;
        public BindableCollection<string> TitleSuggestions { get; set; }
        public BindableCollection<Music> PlaylistItems { get; set; }

        private readonly IEventAggregator _eventAggregator;
        public SelectiveFilterViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _searchesSuggestion = true;
            _allTracks = FileManager.GetAllTrackList().ToList();

            List<Music> playlist = FileManager.Import<SelectiveFilter>(CurrentFilterPath).Playlist;
            PlaylistItems = new BindableCollection<Music>(playlist);
            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;

            Publish();
        }
        protected override void Publish()
        {
            _eventAggregator.PublishOnUIThreadAsync(new SelectiveFilter() { Playlist = PlaylistItems.ToList() });
        }
        public override void ExportFilter()
        {
            var filter = new SelectiveFilter() { Playlist = PlaylistItems.ToList() };
            FileManager.Export(filter, CurrentFilterPath);
        }


        public void AddItem()
        {
            Music item;

            try
            {
                Track track = _allTracks.Find(x => x.Title.Equals(TitleSearchBox));
                string key = StyleSearchBox.ToUpper();
                item = new Music()
                {
                    Title = track.Title,
                    Style = key,
                    Level = track.Patterns[key] > 0 ? null : throw new NullReferenceException()
                };
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is KeyNotFoundException)
                {
                    MessageBox.Show("Pattern does not exist.", "Playlist Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                throw;
            }

            PlaylistItems.Add(item);

            TitleSearchBox = string.Empty;
            StyleSearchBox = string.Empty;

            Publish();
        }
        public void RemoveItem(object item)
        {
            PlaylistItems.Remove(item as Music);
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
                var concat = FileManager.Import<SelectiveFilter>(dialog.FileName).Playlist;
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
                FileManager.Export(selectiveFilter, dialog.FileName);
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
                List<Music> playlist = FileManager.Import<SelectiveFilter>(dialog.FileName).Playlist;
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

            var titles = from track in _allTracks
                         let title = track.Title
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
