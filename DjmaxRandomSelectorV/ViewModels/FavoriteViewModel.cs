using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FavoriteViewModel : Screen
    {
        private const int ItemCountPerPage = 20;
        private readonly IEventAggregator _eventAggregator;
        private readonly TrackDB _db;

        private string _searchBox;
        private string _pageText;
        private string _itemCountText;
        private bool _filtersAllTrack;
        private bool _filtersFavorite;
        private bool _filtersBlacklist;
        private List<FavoriteItem> _items;
        private List<FavoriteItem> _searchedItems;
        private int _pageCount; // 0-indexed
        private int _currentPage;

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange();
                GetSearchedItems();
            }
        }
        public string PageText
        {
            get { return _pageText; }
            set
            {
                _pageText = value;
                NotifyOfPropertyChange();
            }
        }
        public string ItemCountText
        {
            get { return _itemCountText; }
            set
            {
                _itemCountText = value;
                NotifyOfPropertyChange();
            }
        }
        public bool FiltersAllTrack
        {
            get { return _filtersAllTrack; }
            set
            {
                _filtersAllTrack = value;
                NotifyOfPropertyChange();
                GetSearchedItems();
            }
        }
        public bool FiltersFavorite
        {
            get { return _filtersFavorite; }
            set
            {
                _filtersFavorite = value;
                NotifyOfPropertyChange();
                GetSearchedItems();
            }
        }
        public bool FiltersBlacklist
        {
            get { return _filtersBlacklist; }
            set
            {
                _filtersBlacklist = value;
                NotifyOfPropertyChange();
                GetSearchedItems();
            }
        }

        public BindableCollection<FavoriteItem> SearchResult { get; }
        public BindableCollection<FavoriteItem> FavoriteItems { get; }
        public BindableCollection<FavoriteItem> BlacklistItems { get; }

        public FavoriteViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            var setting = IoC.Get<Dmrsv3Configuration>().Setting;
            var favorite = setting.Favorite;
            var blacklist = setting.Blacklist;
            _db = IoC.Get<TrackDB>();
            _items = _db.AllTrack.Select(track => new FavoriteItem()
            {
                Info = track.Info,
                IsPlayable = track.IsPlayable, // TODO: apply the track is playable (required to respond to change setting)
                Status = favorite.Contains(track.Id) ? 1 : (blacklist.Contains(track.Id) ? -1 : 0)
            }).OrderBy(item => item.Title, new TitleComparer()).ToList();
            _searchedItems = new List<FavoriteItem>();
            _filtersAllTrack = true;
            _filtersFavorite = false;
            _filtersBlacklist = false;
            _pageCount = 0;
            _currentPage = 0;
            SearchResult = new BindableCollection<FavoriteItem>();
            GetSearchedItems();
        }

        public void CloseDialog()
        {
            List<int> favorite = _items.Where(item => item.Status == 1).Select(item => item.Id).ToList();
            List<int> blacklist = _items.Where(item => item.Status == -1).Select(item => item.Id).ToList();

            var setting = IoC.Get<Dmrsv3Configuration>().Setting;
            setting.Favorite = favorite;
            setting.Blacklist = blacklist;

            var message = new FavoriteMessage(favorite, blacklist);
            _eventAggregator.PublishOnUIThreadAsync(message);

            TryCloseAsync(true);
        }

        public void CloseDialogWithoutSave() => TryCloseAsync(true);

        public void GetSearchedItems()
        {
            var searched = _items.Select(item => item);
            if (FiltersFavorite)
            {
                searched = searched.Where(item => item.Status == 1);
            }
            else if (FiltersBlacklist)
            {
                searched = searched.Where(item => item.Status == -1);
            }
            if (!string.IsNullOrEmpty(SearchBox))
            {
                searched = SearchBox.Equals("#")
                           ? searched.Where(item => !Regex.IsMatch(item.Title[..1], "[a-z]", RegexOptions.IgnoreCase))
                           : searched.Where(item => item.Title.StartsWith(SearchBox, StringComparison.OrdinalIgnoreCase));
            }
            _searchedItems = searched.ToList();
            _pageCount = _searchedItems.Count / ItemCountPerPage;
            _currentPage = 0;
            UpdateResult();
        }

        public void UpdateResult()
        {
            SearchResult.Clear();
            int start = ItemCountPerPage * _currentPage;
            int count = Math.Min(ItemCountPerPage, _searchedItems.Count - start);
            SearchResult.AddRange(_searchedItems.GetRange(start, count));
            PageText = $"{_currentPage + 1} / {_pageCount + 1}";
            ItemCountText = $"{start+1}-{start+count} / {_searchedItems.Count}";
        }

        public void TurnPage(int toward)
        {
            if (toward == 1 && _currentPage == _pageCount)
            {
                return;
            }
            if (toward == -1 && _currentPage == 0)
            {
                return;
            }
            _currentPage += toward;
            UpdateResult();
        }
        /*
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
                titles = from title in _items.Select(item => item.Info.Title)
                         where !Regex.IsMatch(title[..1], "[a-z]", RegexOptions.IgnoreCase)
                         select title;
            }
            else
            {
                titles = from title in _items.Select(item => item.Info.Title)
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
        */
        public void ClearSearchBox()
        {
            SearchBox = string.Empty;
        }

        public void ChangeItemStatus(FavoriteItem item, int newStatus)
        {
            if (item == null)
            {
                return;
            }
            item.Status = item.Status != newStatus ? newStatus : 0;
            SearchResult.Refresh();
        }
    }
}
