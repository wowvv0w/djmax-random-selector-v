using Caliburn.Micro;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PlaylistViewModel : Screen
    {
        private readonly Playlist _playlist;
        private readonly List<Track> _trackList;


        private bool _searchesSuggestion;
        public BindableCollection<string> TitleSuggestions { get; set; }
        public BindableCollection<Music> PlaylistItems { get; set; }

        public PlaylistViewModel(Playlist playlist, List<Track> trackList)
        {
            _searchesSuggestion = true;
            _playlist = playlist;
            _trackList = trackList;

            PlaylistItems = new BindableCollection<Music>(_playlist.MusicList);
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
                item = new()
                {
                    Title = track.Title,
                    Style = key,
                    Level = track.Patterns[key].ToString()
                };
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is KeyNotFoundException)
                {
                    MessageBox.Show("Does not exist.",
                        "Playlist Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                throw;
            }

            _playlist.MusicList.Add(item);
            PlaylistItems.Add(item);

            TitleSearchBox = string.Empty;
            StyleSearchBox = string.Empty;
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
