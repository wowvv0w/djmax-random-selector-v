using Caliburn.Micro;
using Dmrsv.Data.Context.Schema;
using Dmrsv.Data.Controller;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class QueryFilterViewModel : FilterBaseViewModel
    {
        private QueryFilter _filter;
        private readonly FilterApi _api;

        private readonly FavoriteViewModel _favoriteViewModel;

        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        public QueryFilterViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _api = new FilterApi();

            _filter = _api.GetQueryFilter();
            _filter.Favorites = _api.GetExtraFilter().Favorites;
            _favoriteViewModel = new FavoriteViewModel(_filter.Favorites);
            for(int i = 0; i < 16; i++)
            {
                // DO NOT use index 0
                LevelIndicators.Add(new LevelIndicator());
                ScLevelIndicators.Add(new LevelIndicator());
            }
            UpdateLevelIndicators();
            UpdateScLevelIndicators();

            Publish();
        }

        protected override void Publish()
        {
            _api.SetQueryFilter(_filter);
            _eventAggregator.PublishOnUIThreadAsync(_filter);
        }

        #region Filter Updater
        private bool CheckFilter(List<string> filter, string value)
        {
            return filter.Contains(value);
        }
        private void UpdateFilter(bool isChecked, List<string> filter, string value)
        {
            if (isChecked)
            {
                filter.Add(value);
            }
            else
            {
                filter.Remove(value);
            }
            Publish();
        }
        public void ReloadFilter(string presetPath)
        {
            try
            {
                _filter = _api.GetPreset(presetPath);
                NotifyOfPropertyChange(string.Empty);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Cannot apply the preset.",
                                "Filter Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Publish();
        }
        public void SelectAllCategories()
        {
            _filter.Categories.Clear();
            _filter.Categories.AddRange(new List<string>() { 
                _RP, _P1, _P2, _P3, _TR, _CE, _BS, _VE, _VE2, _ES,
                _T1, _T2, _T3, _TQ, _GG, _CHU, _CY, _DM, _ESTI, _GC, _GF, _MD, _NXN
            });
            _filter.IncludesFavorite = true;
            NotifyOfPropertyChange(string.Empty);
            Publish();
        }
        public void DeselectAllCategories()
        {
            _filter.Categories.Clear();
            _filter.IncludesFavorite = false;
            NotifyOfPropertyChange(string.Empty);
            Publish();
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

        #region Level Indicator
        public class LevelIndicator : PropertyChangedBase
        {
            private bool _value;
            public bool Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    NotifyOfPropertyChange(() => Value);
                }
            }
            public LevelIndicator()
            {
                _value = true;
            }
        }
        public List<LevelIndicator> LevelIndicators { get; set; }
            = new List<LevelIndicator>();
        public List<LevelIndicator> ScLevelIndicators { get; set; }
            = new List<LevelIndicator>();

        public void UpdateLevelIndicators()
        {
            for (int i = 1; i < LevelMin; i++)
            {
                LevelIndicators[i].Value = false;
            }
            for (int i = LevelMin; i <= LevelMax; i++)
            {
                LevelIndicators[i].Value = true;
            }
            for (int i = LevelMax + 1; i <= 15; i++)
            {
                LevelIndicators[i].Value = false;
            }
        }
        public void UpdateScLevelIndicators()
        {
            for (int i = 1; i < ScLevelMin; i++)
            {
                ScLevelIndicators[i].Value = false;
            }
            for (int i = ScLevelMin; i <= ScLevelMax; i++)
            {
                ScLevelIndicators[i].Value = true;
            }
            for (int i = ScLevelMax + 1; i <= 15; i++)
            {
                ScLevelIndicators[i].Value = false;
            }
        }
        #endregion

        #region Tool
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
                _api.SetPreset(_filter, dialog.FileName);
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
                ReloadFilter(dialog.FileName);
        }

        public async void OpenFavoriteEditor()
        {
            bool? result = await _windowManager.ShowDialogAsync(_favoriteViewModel);
            if (result == true)
            {
                Publish();
            }
        }
        #endregion


        #region Filter Elements
        #region Constants
        private const string _4B = "4B";
        private const string _5B = "5B";
        private const string _6B = "6B";
        private const string _8B = "8B";
        private const string _NM = "NM";
        private const string _HD = "HD";
        private const string _MX = "MX";
        private const string _SC = "SC";
        private const string _RP = "RP";
        private const string _P1 = "P1";
        private const string _P2 = "P2";
        private const string _P3 = "P3";
        private const string _TR = "TR";
        private const string _CE = "CE";
        private const string _BS = "BS";
        private const string _VE = "VE";
        private const string _VE2 = "VE2";
        private const string _ES = "ES";
        private const string _T1 = "T1";
        private const string _T2 = "T2";
        private const string _T3 = "T3";
        private const string _TQ = "TQ";
        private const string _GG = "GG";
        private const string _CHU = "CHU";
        private const string _CY = "CY";
        private const string _DM = "DM";
        private const string _ESTI = "ESTI";
        private const string _GC = "GC";
        private const string _GF = "GF";
        private const string _MD = "MD";
        private const string _NXN = "NXN";
        #endregion
        #region ButtonTunes
        public bool ButtonTune4B
        {
            get { return CheckFilter(_filter.ButtonTunes, _4B); }
            set
            {
                UpdateFilter(value, _filter.ButtonTunes, _4B);
                NotifyOfPropertyChange(() => ButtonTune4B);
            }
        }
        public bool ButtonTune5B
        {
            get { return CheckFilter(_filter.ButtonTunes, _5B); }
            set
            {
                UpdateFilter(value, _filter.ButtonTunes, _5B);
                NotifyOfPropertyChange(() => ButtonTune5B);
            }
        }
        public bool ButtonTune6B
        {
            get { return CheckFilter(_filter.ButtonTunes, _6B); }
            set
            {
                UpdateFilter(value, _filter.ButtonTunes, _6B);
                NotifyOfPropertyChange(() => ButtonTune6B);
            }
        }
        public bool ButtonTune8B
        {
            get { return CheckFilter(_filter.ButtonTunes, _8B); }
            set
            {
                UpdateFilter(value, _filter.ButtonTunes, _8B);
                NotifyOfPropertyChange(() => ButtonTune8B);
            }
        }
        #endregion
        #region Difficulty
        public bool Difficulty
        {
            get { return CheckFilter(_filter.Difficulties, _NM); }
            set
            {
                UpdateFilter(value, _filter.Difficulties, _NM);
                UpdateFilter(value, _filter.Difficulties, _HD);
                UpdateFilter(value, _filter.Difficulties, _MX);
                NotifyOfPropertyChange(() => Difficulty);
            }
        }
        public bool DifficultySC
        {
            get { return CheckFilter(_filter.Difficulties, _SC); }
            set
            {
                UpdateFilter(value, _filter.Difficulties, _SC);
                NotifyOfPropertyChange(() => DifficultySC);
            }
        }
        public int LevelMin
        {
            get { return _filter.Levels[0]; }
            set
            {
                _filter.Levels[0] = value;
                NotifyOfPropertyChange(() => LevelMin);
                UpdateLevelIndicators();
            }
        }
        public int LevelMax
        {
            get { return _filter.Levels[1]; }
            set
            {
                _filter.Levels[1] = value;
                NotifyOfPropertyChange(() => LevelMax);
                UpdateLevelIndicators();
            }
        }
        public int ScLevelMin
        {
            get { return _filter.ScLevels[0]; }
            set
            {
                _filter.ScLevels[0] = value;
                NotifyOfPropertyChange(() => ScLevelMin);
                UpdateScLevelIndicators();
            }
        }
        public int ScLevelMax
        {
            get { return _filter.ScLevels[1]; }
            set
            {
                _filter.ScLevels[1] = value;
                NotifyOfPropertyChange(() => ScLevelMax);
                UpdateScLevelIndicators();
            }
        }
        #endregion
        #region Category
        public bool CategoryRP
        {
            get { return CheckFilter(_filter.Categories, _RP); }
            set
            {
                UpdateFilter(value, _filter.Categories, _RP);
                NotifyOfPropertyChange(() => CategoryRP);
            }
        }
        public bool CategoryP1
        {
            get { return CheckFilter(_filter.Categories, _P1); }
            set
            {
                UpdateFilter(value, _filter.Categories, _P1);
                NotifyOfPropertyChange(() => CategoryP1);
            }
        }
        public bool CategoryP2
        {
            get { return CheckFilter(_filter.Categories, _P2); }
            set
            {
                UpdateFilter(value, _filter.Categories, _P2);
                NotifyOfPropertyChange(() => CategoryP2);
            }
        }
        public bool CategoryP3
        {
            get { return CheckFilter(_filter.Categories, _P3); }
            set
            {
                UpdateFilter(value, _filter.Categories, _P3);
                NotifyOfPropertyChange(() => CategoryP3);
            }
        }
        public bool CategoryTR
        {
            get { return CheckFilter(_filter.Categories, _TR); }
            set
            {
                UpdateFilter(value, _filter.Categories, _TR);
                NotifyOfPropertyChange(() => CategoryTR);
            }
        }
        public bool CategoryCE
        {
            get { return CheckFilter(_filter.Categories, _CE); }
            set
            {
                UpdateFilter(value, _filter.Categories, _CE);
                NotifyOfPropertyChange(() => CategoryCE);
            }
        }
        public bool CategoryBS
        {
            get { return CheckFilter(_filter.Categories, _BS); }
            set
            {
                UpdateFilter(value, _filter.Categories, _BS);
                NotifyOfPropertyChange(() => CategoryBS);
            }
        }
        public bool CategoryVE
        {
            get { return CheckFilter(_filter.Categories, _VE); }
            set
            {
                UpdateFilter(value, _filter.Categories, _VE);
                NotifyOfPropertyChange(() => CategoryVE);
            }
        }
        public bool CategoryVE2
        {
            get { return CheckFilter(_filter.Categories, _VE2); }
            set
            {
                UpdateFilter(value, _filter.Categories, _VE2);
                NotifyOfPropertyChange(() => CategoryVE2);
            }
        }
        public bool CategoryES
        {
            get { return CheckFilter(_filter.Categories, _ES); }
            set
            {
                UpdateFilter(value, _filter.Categories, _ES);
                NotifyOfPropertyChange(() => CategoryES);
            }
        }
        public bool CategoryT1
        {
            get { return CheckFilter(_filter.Categories, _T1); }
            set
            {
                UpdateFilter(value, _filter.Categories, _T1);
                NotifyOfPropertyChange(() => CategoryT1);
            }
        }
        public bool CategoryT2
        {
            get { return CheckFilter(_filter.Categories, _T2); }
            set
            {
                UpdateFilter(value, _filter.Categories, _T2);
                NotifyOfPropertyChange(() => CategoryT2);
            }
        }
        public bool CategoryT3
        {
            get { return CheckFilter(_filter.Categories, _T3); }
            set
            {
                UpdateFilter(value, _filter.Categories, _T3);
                NotifyOfPropertyChange(() => CategoryT3);
            }
        }
        public bool CategoryTQ
        {
            get { return CheckFilter(_filter.Categories, _TQ); }
            set
            {
                UpdateFilter(value, _filter.Categories, _TQ);
                NotifyOfPropertyChange(() => CategoryTQ);
            }
        }
        public bool CategoryGG
        {
            get { return CheckFilter(_filter.Categories, _GG); }
            set
            {
                UpdateFilter(value, _filter.Categories, _GG);
                NotifyOfPropertyChange(() => CategoryGG);
            }
        }
        public bool CategoryCHU
        {
            get { return CheckFilter(_filter.Categories, _CHU); }
            set
            {
                UpdateFilter(value, _filter.Categories, _CHU);
                NotifyOfPropertyChange(() => CategoryCHU);
            }
        }
        public bool CategoryCY
        {
            get { return CheckFilter(_filter.Categories, _CY); }
            set
            {
                UpdateFilter(value, _filter.Categories, _CY);
                NotifyOfPropertyChange(() => CategoryCY);
            }
        }
        public bool CategoryDM
        {
            get { return CheckFilter(_filter.Categories, _DM); }
            set
            {
                UpdateFilter(value, _filter.Categories, _DM);
                NotifyOfPropertyChange(() => CategoryDM);
            }
        }
        public bool CategoryESTI
        {
            get { return CheckFilter(_filter.Categories, _ESTI); }
            set
            {
                UpdateFilter(value, _filter.Categories, _ESTI);
                NotifyOfPropertyChange(() => CategoryESTI);
            }
        }
        public bool CategoryGC
        {
            get { return CheckFilter(_filter.Categories, _GC); }
            set
            {
                UpdateFilter(value, _filter.Categories, _GC);
                NotifyOfPropertyChange(() => CategoryGC);
            }
        }
        public bool CategoryGF
        {
            get { return CheckFilter(_filter.Categories, _GF); }
            set
            {
                UpdateFilter(value, _filter.Categories, _GF);
                NotifyOfPropertyChange(() => CategoryGF);
            }
        }
        public bool CategoryMD
        {
            get { return CheckFilter(_filter.Categories, _MD); }
            set
            {
                UpdateFilter(value, _filter.Categories, _MD);
                NotifyOfPropertyChange(() => CategoryMD);
            }
        }
        public bool CategoryNXN
        {
            get { return CheckFilter(_filter.Categories, _NXN); }
            set
            {
                UpdateFilter(value, _filter.Categories, _NXN);
                NotifyOfPropertyChange(() => CategoryNXN);
            }
        }
        #endregion
        public bool CategoryFavorite
        {
            get { return _filter.IncludesFavorite; }
            set
            {
                _filter.IncludesFavorite = value;
                NotifyOfPropertyChange(() => CategoryFavorite);
            }
        }
        #endregion

    }
}
