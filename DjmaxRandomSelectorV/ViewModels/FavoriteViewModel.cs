using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FavoriteViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly TrackDB _db;

        private string _searchBox;
        private List<string> _titleList;

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange();
            }
        }

        public BindableCollection<FavoriteItem> TrackItems { get; }
        public BindableCollection<FavoriteItem> FavoriteItems { get; }
        public BindableCollection<FavoriteItem> BlacklistItems { get; }
        public BindableCollection<string> TitleSuggestions { get; }

        public FavoriteViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _db = IoC.Get<TrackDB>();
            _titleList = _db.AllTrack.Select(t => t.Title).ToList();

            var setting = IoC.Get<Dmrsv3Configuration>().Setting;
            var favorite = setting.Favorite.Where(id => 0 <= id && id < setting.Favorite.Count);
            var blacklist = setting.Blacklist.Where(id => 0 <= id && id < setting.Blacklist.Count);
            FavoriteItems = new BindableCollection<FavoriteItem>(favorite.Select(id =>
            {
                var track = _db.AllTrack[id];
                return new FavoriteItem()
                {
                    Id = id,
                    Title = track.Title,
                    Composer = track.Composer,
                    Category = track.Category,
                    IsPlayable = _db.Playable.Any(t => t.Id == id)
                };
            }));
            BlacklistItems = new BindableCollection<FavoriteItem>(blacklist.Select(id =>
            {
                var track = _db.AllTrack[id];
                return new FavoriteItem()
                {
                    Id = id,
                    Title = track.Title,
                    Composer = track.Composer,
                    Category = track.Category,
                    IsPlayable = _db.Playable.Any(t => t.Id == id)
                };
            }));

            TitleSuggestions = new BindableCollection<string>();
        }
        public void CloseDialog()
        {
            List<int> favorite = FavoriteItems.Select(item => item.Id).ToList();
            List<int> blacklist = BlacklistItems.Select(item => item.Id).ToList();

            var setting = IoC.Get<Dmrsv3Configuration>().Setting;
            setting.Favorite = favorite;
            setting.Blacklist = blacklist;

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
            else if (FavoriteItems.Any(item => item.Title.Equals(SearchBox)) || BlacklistItems.Any(item => item.Title.Equals(SearchBox)))
            {
                MessageBox.Show("Already exists.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Track track = _db.AllTrack.First(t => t.Title.Equals(SearchBox));
            var item = new FavoriteItem(track);
            FavoriteItems.Add(item);

            SearchBox = string.Empty;
        }
        public void RemoveFromFavorite(string child) => FavoriteItems.Remove(FavoriteItems.First(item => item.Title.Equals(child)));
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
            else if (BlacklistItems.Any(item => item.Title.Equals(SearchBox)) || BlacklistItems.Any(item => item.Title.Equals(SearchBox)))
            {
                MessageBox.Show("Already exists.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Track track = _db.AllTrack.First(t => t.Title.Equals(SearchBox));
            var item = new FavoriteItem(track);
            BlacklistItems.Add(item);

            SearchBox = string.Empty;
        }
        public void RemoveFromBlacklist(string child) => BlacklistItems.Remove(BlacklistItems.First(item => item.Title.Equals(child)));
        #endregion
    }
}
