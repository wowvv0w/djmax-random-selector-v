using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
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
        private List<string> _favorite;
        public BindableCollection<string> FavoriteItems { get; set; }

        public BindableCollection<string> TitleSuggestions { get; set; }

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

        private bool _searchesSuggestion = true;
        private string _searchBox;
        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                NotifyOfPropertyChange(() => SearchBox);
                if (_searchesSuggestion) { SearchTitle(); }
            }
        }

        public FavoriteViewModel(List<string> favorite)
        {
            _favorite = favorite;

            FavoriteItems = new BindableCollection<string>(favorite);

            TitleSuggestions = new BindableCollection<string>();
            OpensSuggestionBox = false;
        }

        private void SearchTitle()
        {
            TitleSuggestions.Clear();

            if (string.IsNullOrEmpty(SearchBox))
            {
                OpensSuggestionBox = false;
                return;
            }

            OpensSuggestionBox = true;

            var titles = from track in Selector.AllTrackList
                         where track.Title.StartsWith(SearchBox, true, null)
                         select track.Title;
            titles = titles.Count() < 8 ? titles.Take(titles.Count()) : titles.Take(8);
            TitleSuggestions.AddRange(titles);

            if (TitleSuggestions.Count == 0)
            {
                OpensSuggestionBox = false;
            }
        }

        public void SelectSuggestion(string title)
        {
            _searchesSuggestion = false;

            SearchBox = title;
            OpensSuggestionBox = false;

            _searchesSuggestion = true;
        }

        public void AddItem()
        {
            if (!Selector.AllTrackList.Any(x => x.Title.Equals(SearchBox)))
            {
                MessageBox.Show("Does not exist.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            else if (FavoriteItems.Contains(SearchBox))
            {
                MessageBox.Show("Already exists.",
                    "Favorite Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            FavoriteItems.Add(SearchBox);
            _favorite.Add(SearchBox);

            SearchBox = String.Empty;
            Selector.IsFilterChanged = true;
        }

        public void RemoveItem(string child)
        {
            FavoriteItems.Remove(child);
            _favorite.Remove(child);
            Selector.IsFilterChanged = true;
        }
    }
}
