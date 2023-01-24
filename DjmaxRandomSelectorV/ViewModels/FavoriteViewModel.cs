using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FavoriteViewModel : Screen
    {
        private bool searchesSuggestion;
        private readonly List<string> _titleList;
        //private readonly ExtraFilter _extraFilter;

        public BindableCollection<string> FavoriteItems { get; set; }
        public BindableCollection<string> BlacklistItems { get; set; }
        public BindableCollection<string> TitleSuggestions { get; set; }

        public FavoriteViewModel()
        {
            searchesSuggestion = true;
            //_titleList = new TrackApi().GetAllTrackList().ToList().ConvertAll(x => x.Title);

            //_extraFilter = new FilterApi().GetExtraFilter();

            //FavoriteItems = new BindableCollection<string>(_extraFilter.Favorites);
            //BlacklistItems = new BindableCollection<string>(_extraFilter.Blacklist);

            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;
        }
        public void OK()
        {
            //_extraFilter.Favorites = FavoriteItems.ToList();
            //_extraFilter.Blacklist = BlacklistItems.ToList();
            //new FilterApi().SetExtraFilter(_extraFilter);
            TryCloseAsync(true);
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
                if (searchesSuggestion) { SearchTitle(); }
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
                var titles = from title in _titleList
                             where !Regex.IsMatch(title, "[a-z]", RegexOptions.IgnoreCase)
                             select title;
                TitleSuggestions.AddRange(titles);
            }
            else
            {
                var titles = from title in _titleList
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
            searchesSuggestion = false;

            SearchBox = title;
            OpensSuggestionBox = false;

            searchesSuggestion = true;
        }
        #endregion

        #region Favorite Item Adjustment
        public void AddToFavorite()
        {
            if (!_titleList.Any(x => x.Equals(SearchBox)))
            {
                MessageBox.Show("Does not exist.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            else if (FavoriteItems.Contains(SearchBox) || BlacklistItems.Contains(SearchBox))
            {
                MessageBox.Show("Already exists.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            FavoriteItems.Add(SearchBox);

            SearchBox = string.Empty;
        }
        public void RemoveFromFavorite(string child) => FavoriteItems.Remove(child);
        #endregion

        #region Blacklist Item Adjustment
        public void AddToBlacklist()
        {
            if (!_titleList.Any(x => x.Equals(SearchBox)))
            {
                MessageBox.Show("Does not exist.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            else if (BlacklistItems.Contains(SearchBox) || BlacklistItems.Contains(SearchBox))
            {
                MessageBox.Show("Already exists.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            BlacklistItems.Add(SearchBox);

            SearchBox = string.Empty;
        }
        public void RemoveFromBlacklist(string child) => BlacklistItems.Remove(child);
        #endregion
    }
}
