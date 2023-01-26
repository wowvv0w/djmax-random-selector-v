using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using Dmrsv.RandomSelector;
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
        private readonly IEventAggregator _eventAggregator;
        private bool searchesSuggestion;
        private readonly List<string> _titleList;
        //private readonly ExtraFilter _extraFilter;

        public BindableCollection<string> FavoriteItems { get; set; }
        public BindableCollection<string> BlacklistItems { get; set; }
        public BindableCollection<string> TitleSuggestions { get; set; }

        public FavoriteViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            searchesSuggestion = true;
            _titleList = new TrackManager().GetAllTrack().ConvertAll(x => x.Title);

            var config = IoC.Get<Configuration>();
            FavoriteItems = new BindableCollection<string>(config.Favorite);
            BlacklistItems = new BindableCollection<string>(config.Blacklist);

            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;
        }
        public void CloseDialog()
        {
            List<string> favorite = FavoriteItems.ToList();
            List<string> blacklist = BlacklistItems.ToList();

            var config = IoC.Get<Configuration>();
            config.Favorite = favorite;
            config.Blacklist = blacklist;

            var message = new FavoriteMessage(favorite, blacklist);
            _eventAggregator.PublishOnUIThreadAsync(message);

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
