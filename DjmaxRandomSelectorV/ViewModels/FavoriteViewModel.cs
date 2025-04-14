using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FavoriteViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly TrackDB _db;

        private string _searchBox;
        private List<FavoriteItem> _items;

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange();
                UpdateResult();
            }
        }

        public BindableCollection<FavoriteItem> SearchResult { get; }
        public BindableCollection<FavoriteItem> FavoriteItems { get; }
        public BindableCollection<FavoriteItem> BlacklistItems { get; }
        public BindableCollection<string> TitleSuggestions { get; }

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
            }).OrderBy(item => item.Title).ToList();
            SearchResult = new BindableCollection<FavoriteItem>(_items);
            TitleSuggestions = new BindableCollection<string>();
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

        public void UpdateResult()
        {
            SearchResult.Clear();
            if (string.IsNullOrEmpty(SearchBox))
            {
                SearchResult.AddRange(_items);
            }
            var tracks = SearchBox.Equals("#")
                         ? _items.Where(item => !Regex.IsMatch(item.Title[..1], "[a-z]", RegexOptions.IgnoreCase))
                         : _items.Where(item => item.Info.Title.StartsWith(SearchBox, StringComparison.OrdinalIgnoreCase));
            if (tracks is null)
            {
                return;
            }
            SearchResult.AddRange(tracks);
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
