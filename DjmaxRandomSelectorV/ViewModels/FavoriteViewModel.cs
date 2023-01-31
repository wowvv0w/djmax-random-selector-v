using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FavoriteViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly List<string> _titleList;

        private string _searchBox;

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange();
            }
        }

        public BindableCollection<string> FavoriteItems { get; }
        public BindableCollection<string> BlacklistItems { get; }
        public BindableCollection<string> TitleSuggestions { get; }

        public FavoriteViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _titleList = new TrackManager().GetAllTrack().Select(x => x.Title).ToList();

            var config = IoC.Get<Configuration>();
            FavoriteItems = new BindableCollection<string>(config.Favorite);
            BlacklistItems = new BindableCollection<string>(config.Blacklist);

            TitleSuggestions = new BindableCollection<string>();
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
                titles = from title in _titleList
                         where !Regex.IsMatch(title[..1], "[a-z]", RegexOptions.IgnoreCase)
                         select title;
            }
            else
            {
                titles = from title in _titleList
                         where title.StartsWith(SearchBox, true, null)
                         select title;
            }
            TitleSuggestions.AddRange(titles);
        }

        public void SelectSuggestion(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return;
            }
            SearchBox = title;
        }

        public void ClearSearchBox()
        {
            SearchBox = string.Empty;
        }

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
