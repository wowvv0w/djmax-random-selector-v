using Caliburn.Micro;
using DjmaxRandomSelectorV.Messages;
using DjmaxRandomSelectorV.Models;
using Dmrsv.RandomSelector;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class QueryFilterViewModel : CategoryContainer
    {
        private const string DefaultPath = @"Data\CurrentFilter.json";
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;

        private QueryFilter _filter;

        public BindableCollection<ListUpdater> ButtonTunesUpdaters { get; set; }
        public BindableCollection<ListUpdater> RegularCategories { get; set; }
        public BindableCollection<ListUpdater> CollabCategories { get; set; }
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

        public QueryFilterViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, IFileManager fileManager)
        {
            DisplayName = "FILTER";
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _fileManager = fileManager;

            _categories.Insert(15, new Category("FAVORITE", "FAVORITE", null));
            _categories.Insert(16, new Category("COLLABORATION", null, null));

            ImportFilter(DefaultPath);
            Initialize();
        }

        public override void Refresh()
        {
            var children = new INotifyPropertyChangedEx[]
            {
                ButtonTunesUpdaters, RegularCategories, CollabCategories,
                LevelIndicators, ScLevelIndicators
            };
            Array.ForEach(children, x => x.Refresh());
            base.Refresh();
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (close)
            {
                _fileManager.Export(_filter, DefaultPath);
            }
            return Task.CompletedTask;
        }

        public void Initialize()
        {
            var buttons = new List<string>() { "4B", "5B", "6B", "8B" }.ConvertAll(x => new ListUpdater(x, x, _filter.ButtonTunes));
            ButtonTunesUpdaters = new BindableCollection<ListUpdater>(buttons);

            InitializeCategoryUpdaters(_filter.Categories);
            var updaters = CategoryUpdaters.ToList();
            RegularCategories = new BindableCollection<ListUpdater>(updaters.GetRange(0, 16));
            CollabCategories = new BindableCollection<ListUpdater>(updaters.GetRange(16, 10));

            LevelIndicators = new BindableCollection<LevelIndicator>();
            ScLevelIndicators = new BindableCollection<LevelIndicator>();
            for (int i = 1; i <= 15; i++)
            {
                LevelIndicators.Add(new LevelIndicator(i, _filter.Levels));
                ScLevelIndicators.Add(new LevelIndicator(i, _filter.ScLevels));
            }
        }

        private void ImportFilter(string path)
        {
            _filter = _fileManager.Import<QueryFilter>(path);
            var config = IoC.Get<Configuration>();
            _filter.Favorites = config.Favorite;
            _filter.Blacklist = config.Blacklist;
            _eventAggregator.PublishOnUIThreadAsync(new FilterMessage(_filter));
        }

        public void ReloadFilter(string presetPath)
        {
            try
            {
                ImportFilter(presetPath);
                Initialize();
                Refresh();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Cannot apply the preset.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SelectAllCategories()
        {
            _filter.Categories.Clear();
            foreach (var c in _categories.ConvertAll(x => x.Id).Where(id => !string.IsNullOrEmpty(id)))
            {
                _filter.Categories.Add(c);
            }
            RegularCategories.Refresh();
            CollabCategories.Refresh();
        }
        public void DeselectAllCategories()
        {
            _filter.Categories.Clear();
            RegularCategories.Refresh();
            CollabCategories.Refresh();
        }

        public void SavePreset()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, @"Data\Preset");
            var dialog = new SaveFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                _fileManager.Export(_filter, dialog.FileName);
            }
        }
        public void LoadPreset()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, @"Data\Preset");
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                ReloadFilter(dialog.FileName);
            }
        }

        public async void OpenFavoriteEditor()
        {
            bool? result = await _windowManager.ShowDialogAsync(IoC.Get<FavoriteViewModel>());
            if (result == true)
            {
            }
        }

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
