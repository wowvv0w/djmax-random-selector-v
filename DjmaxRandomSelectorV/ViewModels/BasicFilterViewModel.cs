using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using DjmaxRandomSelectorV.SerializableObjects;
using DjmaxRandomSelectorV.Services;
using DjmaxRandomSelectorV.States;
using Microsoft.Win32;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class BasicFilterViewModel : Screen
    {
        private const string DefaultPath = @"DMRSV3_Data\CurrentFilter.json";
        private const string PresetPath = @"DMRSV3_Data\Preset";

        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;
        private readonly IFilterStateManager _filterManager;
        private readonly ISettingStateManager _settingManager;
        private readonly IReadOnlyList<Category> _categories;

        private BasicFilter _filter;

        #region Filter Editor
        public BindableCollection<ListUpdater> ButtonTunesUpdaters { get; set; }
        public BindableCollection<ListUpdater> RegularCategories { get; set; }
        public BindableCollection<ListUpdater> CollabCategories { get; set; }
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
            IFilterStateManager filterManager, ISettingStateManager settingManager)
        {
            DisplayName = "FILTER";
            _windowManager = windowManager;
            _fileManager = fileManager;
            _filterManager = filterManager;
            _settingManager = settingManager;

            _categories = IoC.Get<ITrackDB>().Categories;

            try
            {
                ImportFilter(DefaultPath);
            }
            catch
            {
                _filter = new BasicFilter(settingManager);
            }
            Initialize();
        }

        public void Initialize()
        {
            var buttons = new List<string>() { "4B", "5B", "6B", "8B" }.ConvertAll(x => new ListUpdater(x, x, _filter.ButtonTunes));
            ButtonTunesUpdaters = new BindableCollection<ListUpdater>(buttons);

            var updatersRegular = _categories.Where(cat => cat.Type == 0).Select(cat => new ListUpdater(cat.Name, cat.Id, _filter.Categories));
            var updatersNotRegular = _categories.Where(cat => cat.Type != 0).Select(cat => new ListUpdater(cat.Name, cat.Id, _filter.Categories));
            RegularCategories = new BindableCollection<ListUpdater>(updatersRegular);
            CollabCategories = new BindableCollection<ListUpdater>(updatersNotRegular);

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

        private void ImportFilter(string path)
        {
            var preset = _fileManager.Import<Dmrsv3BasicFilterPreset>(path);
            _filter = new BasicFilter(preset, _settingManager);
            _filterManager.RegisterFilterState(_filter);
        }

        #region Category Selector
        public void SelectAllCategories()
        {
            _filter.Categories.Clear();
            foreach (var c in _categories.Select(cat => cat.Id).Where(id => !string.IsNullOrEmpty(id)))
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
        #endregion

        public void SavePreset()
        {
            string app = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(app, PresetPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
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
            string path = Path.Combine(app, PresetPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    ImportFilter(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot apply the preset.\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Initialize();
                Refresh();
            }
        }

        public Task OpenFavoriteEditor()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<FavoriteViewModel>());
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
