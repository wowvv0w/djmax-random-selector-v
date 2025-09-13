using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class BasicFilterViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;
        private readonly IFilterStateManager _filterManager;
        private readonly IReadOnlyList<Dmrsv3Category> _categories;

        private BasicFilter _filter;

        #region Filter Editor
        public BindableCollection<PropertyChangedNotifier<bool>> ButtonTunesUpdaters { get; set; }
        public BindableCollection<PropertyChangedNotifier<bool>> RegularCategories { get; set; }
        public BindableCollection<PropertyChangedNotifier<bool>> CollabCategories { get; set; }

        public bool IsFavoriteContained
        {
            get => _filter.IncludesFavorite;
            set
            {
                _filter.IncludesFavorite = value;
                NotifyOfPropertyChange();
            }
        }
        public int LevelMin
        {
            get { return _filter.Levels[0]; }
            set
            {
                _filter.Levels[0] = value;
                NotifyOfPropertyChange();
                LevelIndicators.Refresh();
            }
        }
        public int LevelMax
        {
            get { return _filter.Levels[1]; }
            set
            {
                _filter.Levels[1] = value;
                NotifyOfPropertyChange();
                LevelIndicators.Refresh();
            }
        }
        public int ScLevelMin
        {
            get { return _filter.ScLevels[0]; }
            set
            {
                _filter.ScLevels[0] = value;
                NotifyOfPropertyChange();
                ScLevelIndicators.Refresh();
            }
        }
        public int ScLevelMax
        {
            get { return _filter.ScLevels[1]; }
            set
            {
                _filter.ScLevels[1] = value;
                NotifyOfPropertyChange();
                ScLevelIndicators.Refresh();
            }
        }
        public bool IsDifficultyContained
        {
            get => _filter.Difficulties.Contains("NM");
            set
            {
                string[] difficulties = new string[] { "NM", "HD", "MX" };
                if (value)
                {
                    foreach (var d in difficulties)
                    {
                        _filter.Difficulties.Add(d);
                    }
                }
                else
                {
                    foreach (var d in difficulties)
                    {
                        _filter.Difficulties.Remove(d);
                    }
                }
                NotifyOfPropertyChange();
            }
        }
        public bool IsScContained
        {
            get => _filter.Difficulties.Contains("SC");
            set
            {
                if (value)
                {
                    _filter.Difficulties.Add("SC");
                }
                else
                {
                    _filter.Difficulties.Remove("SC");
                }
            }
        }
        public BindableCollection<LevelIndicator> LevelIndicators { get; set; }
        public BindableCollection<LevelIndicator> ScLevelIndicators { get; set; }
        #endregion

        public BasicFilterViewModel(IWindowManager windowManager, IFileManager fileManager,
            IFilterStateManager filterManager, ITrackDB trackDB)
        {
            DisplayName = "FILTER";
            _windowManager = windowManager;
            _fileManager = fileManager;
            _filterManager = filterManager;

            _categories = trackDB.Categories;
            _filter = new BasicFilter();

            ImportFilter(false);
            Initialize();
        }

        public void Initialize()
        {
            PropertyChangedNotifier<bool> MakeNotifier(string name, string val, ICollection<string> collection)
            {
                return new PropertyChangedNotifier<bool>(
                    name,
                    () => collection.Contains(val),
                    isChecked =>
                    {
                        if (isChecked)
                        {
                            collection.Add(val);
                        }
                        else
                        {
                            collection.Remove(val);
                        }
                    });
            }

            var buttons = new List<string>() { "4B", "5B", "6B", "8B" }.ConvertAll(bt => MakeNotifier(bt, bt, _filter.ButtonTunes));
            ButtonTunesUpdaters = new BindableCollection<PropertyChangedNotifier<bool>>(buttons);

            var categories = _categories
                .GroupBy(cat => cat.Type == 0)
                .OrderByDescending(g => g.Key)
                .Select(g => g.Select(cat => MakeNotifier(cat.Name, cat.Id, _filter.Categories)))
                .ToList();
            RegularCategories = new BindableCollection<PropertyChangedNotifier<bool>>(categories[0]);
            CollabCategories = new BindableCollection<PropertyChangedNotifier<bool>>(categories[1]);

            LevelIndicators = new BindableCollection<LevelIndicator>();
            ScLevelIndicators = new BindableCollection<LevelIndicator>();
            for (int i = 1; i <= 15; i++)
            {
                LevelIndicators.Add(new LevelIndicator(i, _filter.Levels));
                ScLevelIndicators.Add(new LevelIndicator(i, _filter.ScLevels));
            }
        }

        public override void Refresh()
        {
            var children = new INotifyPropertyChangedEx[]
            {
                ButtonTunesUpdaters,
                RegularCategories,
                CollabCategories,
                LevelIndicators,
                ScLevelIndicators
            };
            Array.ForEach(children, child => child.Refresh());
            base.Refresh();
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                ExportFilter(false);
            }
            return Task.CompletedTask;
        }

        #region Helper Functions
        private void ImportFilter(bool useDialog)
        {
            try
            {
                var preset = _fileManager.Load<Dmrsv3BasicFilterPreset>(useDialog);
                _filter = new BasicFilter(preset);
                _filterManager.RegisterFilterState(_filter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot apply the preset.\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportFilter(bool useDialog)
        {
            _fileManager.Save(_filter.ToPreset(), useDialog);
        }
        #endregion

        #region Category Selector
        public void SelectAllCategories()
        {
            _filter.Categories.Clear();
            foreach (var c in _categories.Select(cat => cat.Id).Where(id => !string.IsNullOrEmpty(id)))
            {
                _filter.Categories.Add(c);
            }
            Refresh();
        }

        public void DeselectAllCategories()
        {
            _filter.Categories.Clear();
            Refresh();
        }
        #endregion

        #region Tools
        public void SavePreset()
        {
            ExportFilter(false);
        }

        public void LoadPreset()
        {
            ImportFilter(true);
            Initialize();
            Refresh();
        }

        public Task OpenFavoriteEditor()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<FavoriteViewModel>());
        }
        #endregion

        #region Level Adjustment
        public void IncreaseLevelMin()
        {
            if (LevelMin < 15 && LevelMin < LevelMax)
            {
                LevelMin++;
            }
        }
        public void DecreaseLevelMin()
        {
            if (LevelMin > 1)
            {
                LevelMin--;
            }
        }
        public void IncreaseLevelMax()
        {
            if (LevelMax < 15)
            {
                LevelMax++;
            }
        }
        public void DecreaseLevelMax()
        {
            if (LevelMax > 1 && LevelMax > LevelMin)
            {
                LevelMax--;
            }
        }
        public void IncreaseScLevelMin()
        {
            if (ScLevelMin < 15 && ScLevelMin < ScLevelMax)
            {
                ScLevelMin++;
            }
        }
        public void DecreaseScLevelMin()
        {
            if (ScLevelMin > 1)
            {
                ScLevelMin--;
            }
        }
        public void IncreaseScLevelMax()
        {
            if (ScLevelMax < 15)
            {
                ScLevelMax++;
            }
        }
        public void DecreaseScLevelMax()
        {
            if (ScLevelMax > 1 && ScLevelMax > ScLevelMin)
            {
                ScLevelMax--;
            }
        }
        #endregion
    }
}
