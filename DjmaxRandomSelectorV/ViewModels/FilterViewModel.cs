using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class FilterViewModel : Screen
    {
        public Filter Filter { get; set; }
        public FilterViewModel()
        {
            Filter = new Filter();
            Filter.Import();
            for(int i = 0; i < 16; i++)
            {
                // DO NOT use index 0
                LevelIndicators.Add(new LevelIndicator());
            }
            UpdateLevelIndicators();
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
            Filter.IsUpdated = true;
        }
        public void ReloadFilter(string presetName)
        {
            try
            {
                Filter.Import(presetName);
                NotifyOfPropertyChange(string.Empty);
                MessageBox.Show($"Preset {presetName} has been applied.",
                                "Filter",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                Filter.IsUpdated = true;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Cannot find Preset {presetName}.",
                                "Filter",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
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
        #endregion

        #region Tool
        public void AddPreset()
        {
            string presetName = Microsoft.VisualBasic.Interaction.InputBox("Preset Name: ", "Add Preset");
            if (!string.IsNullOrEmpty(presetName))
            {
                Filter.Export(presetName);
                MessageBox.Show($"Preset {presetName} has been added.",
                                "Preset",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
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
            get { return CheckFilter(Filter.ButtonTunes, _4B); }
            set
            {
                UpdateFilter(value, Filter.ButtonTunes, _4B);
                NotifyOfPropertyChange(() => ButtonTune4B);
            }
        }
        public bool ButtonTune5B
        {
            get { return CheckFilter(Filter.ButtonTunes, _5B); }
            set
            {
                UpdateFilter(value, Filter.ButtonTunes, _5B);
                NotifyOfPropertyChange(() => ButtonTune5B);
            }
        }
        public bool ButtonTune6B
        {
            get { return CheckFilter(Filter.ButtonTunes, _6B); }
            set
            {
                UpdateFilter(value, Filter.ButtonTunes, _6B);
                NotifyOfPropertyChange(() => ButtonTune6B);
            }
        }
        public bool ButtonTune8B
        {
            get { return CheckFilter(Filter.ButtonTunes, _8B); }
            set
            {
                UpdateFilter(value, Filter.ButtonTunes, _8B);
                NotifyOfPropertyChange(() => ButtonTune8B);
            }
        }
        #endregion
        #region Difficulty
        public bool DifficultyNM
        {
            get { return CheckFilter(Filter.Difficulties, _NM); }
            set
            {
                UpdateFilter(value, Filter.Difficulties, _NM);
                NotifyOfPropertyChange(() => DifficultyNM);
            }
        }
        public bool DifficultyHD
        {
            get { return CheckFilter(Filter.Difficulties, _HD); }
            set
            {
                UpdateFilter(value, Filter.Difficulties, _HD);
                NotifyOfPropertyChange(() => DifficultyHD);
            }
        }
        public bool DifficultyMX
        {
            get { return CheckFilter(Filter.Difficulties, _MX); }
            set
            {
                UpdateFilter(value, Filter.Difficulties, _MX);
                NotifyOfPropertyChange(() => DifficultyMX);
            }
        }
        public bool DifficultySC
        {
            get { return CheckFilter(Filter.Difficulties, _SC); }
            set
            {
                UpdateFilter(value, Filter.Difficulties, _SC);
                NotifyOfPropertyChange(() => DifficultySC);
            }
        }
        public int LevelMin
        {
            get { return Filter.Levels[0]; }
            set
            {
                Filter.Levels[0] = value;
                NotifyOfPropertyChange(() => LevelMin);
                UpdateLevelIndicators();
                Filter.IsUpdated = true;
            }
        }
        public int LevelMax
        {
            get { return Filter.Levels[1]; }
            set
            {
                Filter.Levels[1] = value;
                NotifyOfPropertyChange(() => LevelMax);
                UpdateLevelIndicators();
                Filter.IsUpdated = true;
            }
        }
        public int ScLevelMin
        {
            get { return Filter.ScLevels[0]; }
            set
            {
                Filter.ScLevels[0] = value;
                NotifyOfPropertyChange(() => ScLevelMin);
                UpdateLevelIndicators();
                Filter.IsUpdated = true;
            }
        }
        public int ScLevelMax
        {
            get { return Filter.ScLevels[1]; }
            set
            {
                Filter.ScLevels[1] = value;
                NotifyOfPropertyChange(() => ScLevelMax);
                UpdateLevelIndicators();
                Filter.IsUpdated = true;
            }
        }
        #endregion
        #region Category
        public bool CategoryRP
        {
            get { return CheckFilter(Filter.Categories, _RP); }
            set
            {
                UpdateFilter(value, Filter.Categories, _RP);
                NotifyOfPropertyChange(() => CategoryRP);
            }
        }
        public bool CategoryP1
        {
            get { return CheckFilter(Filter.Categories, _P1); }
            set
            {
                UpdateFilter(value, Filter.Categories, _P1);
                NotifyOfPropertyChange(() => CategoryP1);
            }
        }
        public bool CategoryP2
        {
            get { return CheckFilter(Filter.Categories, _P2); }
            set
            {
                UpdateFilter(value, Filter.Categories, _P2);
                NotifyOfPropertyChange(() => CategoryP2);
            }
        }
        public bool CategoryP3
        {
            get { return CheckFilter(Filter.Categories, _P3); }
            set
            {
                UpdateFilter(value, Filter.Categories, _P3);
                NotifyOfPropertyChange(() => CategoryP3);
            }
        }
        public bool CategoryTR
        {
            get { return CheckFilter(Filter.Categories, _TR); }
            set
            {
                UpdateFilter(value, Filter.Categories, _TR);
                NotifyOfPropertyChange(() => CategoryTR);
            }
        }
        public bool CategoryCE
        {
            get { return CheckFilter(Filter.Categories, _CE); }
            set
            {
                UpdateFilter(value, Filter.Categories, _CE);
                NotifyOfPropertyChange(() => CategoryCE);
            }
        }
        public bool CategoryBS
        {
            get { return CheckFilter(Filter.Categories, _BS); }
            set
            {
                UpdateFilter(value, Filter.Categories, _BS);
                NotifyOfPropertyChange(() => CategoryBS);
            }
        }
        public bool CategoryVE
        {
            get { return CheckFilter(Filter.Categories, _VE); }
            set
            {
                UpdateFilter(value, Filter.Categories, _VE);
                NotifyOfPropertyChange(() => CategoryVE);
            }
        }
        public bool CategoryVE2
        {
            get { return CheckFilter(Filter.Categories, _VE2); }
            set
            {
                UpdateFilter(value, Filter.Categories, _VE2);
                NotifyOfPropertyChange(() => CategoryVE2);
            }
        }
        public bool CategoryES
        {
            get { return CheckFilter(Filter.Categories, _ES); }
            set
            {
                UpdateFilter(value, Filter.Categories, _ES);
                NotifyOfPropertyChange(() => CategoryES);
            }
        }
        public bool CategoryT1
        {
            get { return CheckFilter(Filter.Categories, _T1); }
            set
            {
                UpdateFilter(value, Filter.Categories, _T1);
                NotifyOfPropertyChange(() => CategoryT1);
            }
        }
        public bool CategoryT2
        {
            get { return CheckFilter(Filter.Categories, _T2); }
            set
            {
                UpdateFilter(value, Filter.Categories, _T2);
                NotifyOfPropertyChange(() => CategoryT2);
            }
        }
        public bool CategoryT3
        {
            get { return CheckFilter(Filter.Categories, _T3); }
            set
            {
                UpdateFilter(value, Filter.Categories, _T3);
                NotifyOfPropertyChange(() => CategoryT3);
            }
        }
        public bool CategoryGG
        {
            get { return CheckFilter(Filter.Categories, _GG); }
            set
            {
                UpdateFilter(value, Filter.Categories, _GG);
                NotifyOfPropertyChange(() => CategoryGG);
            }
        }
        public bool CategoryCHU
        {
            get { return CheckFilter(Filter.Categories, _CHU); }
            set
            {
                UpdateFilter(value, Filter.Categories, _CHU);
                NotifyOfPropertyChange(() => CategoryCHU);
            }
        }
        public bool CategoryCY
        {
            get { return CheckFilter(Filter.Categories, _CY); }
            set
            {
                UpdateFilter(value, Filter.Categories, _CY);
                NotifyOfPropertyChange(() => CategoryCY);
            }
        }
        public bool CategoryDM
        {
            get { return CheckFilter(Filter.Categories, _DM); }
            set
            {
                UpdateFilter(value, Filter.Categories, _DM);
                NotifyOfPropertyChange(() => CategoryDM);
            }
        }
        public bool CategoryESTI
        {
            get { return CheckFilter(Filter.Categories, _ESTI); }
            set
            {
                UpdateFilter(value, Filter.Categories, _ESTI);
                NotifyOfPropertyChange(() => CategoryESTI);
            }
        }
        public bool CategoryGC
        {
            get { return CheckFilter(Filter.Categories, _GC); }
            set
            {
                UpdateFilter(value, Filter.Categories, _GC);
                NotifyOfPropertyChange(() => CategoryGC);
            }
        }
        public bool CategoryGF
        {
            get { return CheckFilter(Filter.Categories, _GF); }
            set
            {
                UpdateFilter(value, Filter.Categories, _GF);
                NotifyOfPropertyChange(() => CategoryGF);
            }
        }
        public bool CategoryMD
        {
            get { return CheckFilter(Filter.Categories, _MD); }
            set
            {
                UpdateFilter(value, Filter.Categories, _MD);
                NotifyOfPropertyChange(() => CategoryMD);
            }
        }
        public bool CategoryNXN
        {
            get { return CheckFilter(Filter.Categories, _NXN); }
            set
            {
                UpdateFilter(value, Filter.Categories, _NXN);
                NotifyOfPropertyChange(() => CategoryNXN);
            }
        }
        #endregion
        public bool CategoryFavorite
        {
            get { return Filter.IncludesFavorite; }
            set
            {
                Filter.IncludesFavorite = value;
                NotifyOfPropertyChange(() => CategoryFavorite);
                Filter.IsUpdated = true;
            }
        }
        #endregion

    }
}
