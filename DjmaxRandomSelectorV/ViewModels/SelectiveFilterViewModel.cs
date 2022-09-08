using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class SelectiveFilterViewModel : Screen
    {
        private readonly List<Track> _trackList;

        private bool _searchesSuggestion;
        public BindableCollection<string> TitleSuggestions { get; set; }
        public BindableCollection<Music> PlaylistItems { get; set; }

        public SelectiveFilterViewModel()
        {
            _searchesSuggestion = true;
            //_trackList = trackList;

            List<Music> playlist = FileManager.Import<SelectiveFilter>("Data/CurrentPlaylist.json").Playlist;
            PlaylistItems = new BindableCollection<Music>(playlist);
            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;
        }

        public void AddItem()
        {
            Music item;

            try
            {
                Track track = _trackList.Find(x => x.Title.Equals(TitleSearchBox));
                string key = StyleSearchBox.ToUpper();
                item = new Music()
                {
                    Title = track.Title,
                    Style = key,
                    Level = track.Patterns[key] > 0 ? track.Patterns[key].ToString() : throw new NullReferenceException()
                };
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is KeyNotFoundException)
                {
                    MessageBox.Show("Pattern does not exist.",
                        "Playlist Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                throw;
            }

            PlaylistItems.Add(item);

            TitleSearchBox = string.Empty;
            StyleSearchBox = string.Empty;
        }
        public void RemoveItem(object item) => PlaylistItems.Remove(item as Music);
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

            var titles = from track in _trackList
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
